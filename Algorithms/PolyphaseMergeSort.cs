using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WpfApp.Models.Steps;

namespace WpfApp.Algorithms
{
    public class PolyphaseMergeSort : IExternalSortingAlgorithm<string>
    {
        public string Name => "Polyphase Merge Sort";
        public string Description => "External merge sort using multiple tapes for sorting large datasets.";

        private readonly int numberOfTapes;
        private readonly List<string> tapePaths;
        private IComparer<string> comparer;

        public PolyphaseMergeSort(int numberOfTapes)
        {
            if (numberOfTapes < 3)
                throw new ArgumentException("At least 3 tapes are required for polyphase merge sort.");
            
            this.numberOfTapes = numberOfTapes;
            this.tapePaths = new List<string>();
        }

        public IEnumerable<SortingStep<string>> Sort(string inputFilePath, string outputFilePath, IComparer<string> comparer)
        {
            this.comparer = comparer;
            InitializeTapes();

            try
            {
                // Phase 1: Initial Distribution
                foreach (var step in InitialDistribution(inputFilePath, outputFilePath))
                    yield return step;

                // Phase 2: Merge until sorted
                while (true)
                {
                    // Find tapes with data and empty tapes
                    var inputTapes = GetTapesWithData();
                    if (inputTapes.Count <= 1)
                        break; // Sorting is complete

                    // Get all available empty tapes for output
                    var emptyTapes = new List<int>();
                    for (int i = 0; i < numberOfTapes; i++)
                    {
                        if (!inputTapes.Contains(i))
                        {
                            emptyTapes.Add(i);
                        }
                    }

                    if (emptyTapes.Count == 0)
                    {
                        // If no empty tapes, use half of the input tapes as output
                        int tapesToUse = inputTapes.Count / 2;
                        for (int i = 0; i < tapesToUse; i++)
                        {
                            var tape = inputTapes[inputTapes.Count - 1 - i];
                            inputTapes.RemoveAt(inputTapes.Count - 1 - i);
                            emptyTapes.Add(tape);
                            File.WriteAllText(tapePaths[tape], string.Empty);
                        }
                    }

                    // Merge phase
                    foreach (var step in MergePhase(inputTapes, emptyTapes))
                        yield return step;
                }

                // Copy final result to output file
                var finalTape = GetTapesWithData().Single();
                File.Copy(tapePaths[finalTape], outputFilePath, true);
            }
            finally
            {
                CleanupTapes();
            }
        }

        private void InitializeTapes()
        {
            tapePaths.Clear();
            for (int i = 0; i < numberOfTapes; i++)
            {
                string tapePath = Path.Combine(Path.GetTempPath(), $"tape_{i}.tmp");
                tapePaths.Add(tapePath);
                // Create or clear the file
                File.WriteAllText(tapePath, string.Empty);
            }
        }

        private IEnumerable<SortingStep<string>> InitialDistribution(string inputFilePath, string outputFilePath)
        {
            // Create initial sorted runs and distribute them across tapes
            using var reader = new StreamReader(inputFilePath);
            var writers = new List<StreamWriter>();
            
            try
            {
                // Initialize writers for all tapes except the last one (reserved for merging)
                for (int i = 0; i < numberOfTapes - 1; i++)
                {
                    writers.Add(new StreamWriter(tapePaths[i], false));  // false to overwrite
                }

                int currentTape = 0;
                string currentLine;
                string previousLine = null;
                int totalLines = 0;
                
                while ((currentLine = reader.ReadLine()) != null)
                {
                    totalLines++;
                    // Start new run if necessary
                    if (previousLine != null && comparer.Compare(currentLine, previousLine) < 0)
                    {
                        currentTape = (currentTape + 1) % (numberOfTapes - 1);
                    }

                    writers[currentTape].WriteLine(currentLine);
                    writers[currentTape].Flush();  // Ensure data is written immediately
                    yield return new PolyphaseDistributionStep<string>(1, currentTape + 1, currentLine);
                    previousLine = currentLine;
                }

                // If no data was read, create an empty output file
                if (totalLines == 0)
                {
                    File.WriteAllText(outputFilePath, string.Empty);
                    yield break;
                }
            }
            finally
            {
                foreach (var writer in writers)
                {
                    writer?.Dispose();
                }
            }
        }

        private IEnumerable<SortingStep<string>> MergePhase(List<int> inputTapes, List<int> outputTapes)
        {
            if (inputTapes.Count == 0) yield break;

            var readers = new List<StreamReader>();
            var writers = new List<StreamWriter>();
            var currentElements = new List<(string Value, int TapeIndex, int Position)>();
            int outputPosition = 0;
            int currentOutputTape = 0;

            try
            {
                // Open readers for input tapes
                for (int i = 0; i < inputTapes.Count; i++)
                {
                    var reader = new StreamReader(tapePaths[inputTapes[i]]);
                    readers.Add(reader);
                    
                    string firstLine = reader.ReadLine();
                    if (firstLine != null)
                    {
                        currentElements.Add((firstLine, i, 0));
                    }
                }

                // Open writers for output tapes
                foreach (var tape in outputTapes)
                {
                    writers.Add(new StreamWriter(tapePaths[tape], false));
                }

                string lastWritten = null;
                while (currentElements.Count > 0)
                {
                    // Find minimum element
                    var minElementIndex = 0;
                    for (int i = 1; i < currentElements.Count; i++)
                    {
                        if (comparer.Compare(currentElements[i].Value, currentElements[minElementIndex].Value) < 0)
                        {
                            minElementIndex = i;
                        }
                    }

                    var (value, tapeIndex, position) = currentElements[minElementIndex];

                    // Check if we need to start a new run
                    if (lastWritten != null && comparer.Compare(value, lastWritten) < 0)
                    {
                        // Switch to next output tape
                        currentOutputTape = (currentOutputTape + 1) % writers.Count;
                        outputPosition = 0;
                    }

                    // Write the element
                    writers[currentOutputTape].WriteLine(value);
                    writers[currentOutputTape].Flush();
                    yield return new PolyphaseMergeStep<string>(
                        inputTapes[tapeIndex] + 1,
                        outputTapes[currentOutputTape] + 1,
                        position,
                        outputPosition++,
                        value);

                    lastWritten = value;

                    // Get next element from the same tape
                    string nextElement = readers[tapeIndex].ReadLine();
                    if (nextElement != null)
                    {
                        currentElements[minElementIndex] = (nextElement, tapeIndex, position + 1);
                    }
                    else
                    {
                        currentElements.RemoveAt(minElementIndex);
                    }
                }
            }
            finally
            {
                foreach (var reader in readers)
                {
                    reader?.Dispose();
                }
                foreach (var writer in writers)
                {
                    writer?.Dispose();
                }
            }
        }

        private List<int> GetTapesWithData()
        {
            var result = new List<int>();
            for (int i = 0; i < numberOfTapes; i++)
            {
                try
                {
                    if (File.Exists(tapePaths[i]) && new FileInfo(tapePaths[i]).Length > 0)
                    {
                        result.Add(i);
                    }
                }
                catch
                {
                    // Skip this tape if there's an error accessing it
                }
            }
            return result;
        }

        private int GetEmptyTapeIndex()
        {
            for (int i = 0; i < numberOfTapes; i++)
            {
                try
                {
                    if (!File.Exists(tapePaths[i]) || new FileInfo(tapePaths[i]).Length == 0)
                    {
                        return i;
                    }
                }
                catch
                {
                    // Skip this tape if there's an error accessing it
                }
            }
            return -1;
        }

        private void CleanupTapes()
        {
            foreach (var tapePath in tapePaths)
            {
                try
                {
                    if (File.Exists(tapePath))
                        File.Delete(tapePath);
                }
                catch { /* Ignore cleanup errors */ }
            }
        }
    }
}
