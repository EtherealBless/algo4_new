using System;
using System.Collections.Generic;
using System.IO;
using WpfApp.Models.Steps;

namespace WpfApp.Algorithms
{
    public class ExternalMergeSort : ExternalMergeSortBase
    {
        public override string Name => "External Merge Sort";
        public override string Description => "A divide-and-conquer algorithm that sorts large files by splitting them into smaller chunks, sorting each chunk, and merging them back together.";

        private readonly int chunkSize;  // Number of lines to read into memory at once

        public ExternalMergeSort(int chunkSizeInLines = 1000)
        {
            this.chunkSize = chunkSizeInLines;
        }

        public override IEnumerable<SortingStep<string>> Sort(string inputFilePath, string outputFilePath, IComparer<string> comparer)
        {
            try
            {
                // Phase 1: Split into sorted chunks
                var chunkFiles = new List<string>();
                yield return new AlgorithmStatusStep<string>(false, "Starting initial split phase");

                using (var reader = new StreamReader(inputFilePath))
                {
                    List<string> chunk = new();
                    string? line;
                    int chunkNumber = 0;

                    while ((line = reader.ReadLine()) != null)
                    {
                        chunk.Add(line);
                        
                        if (chunk.Count >= chunkSize)
                        {
                            var chunkFile = CreateSortedChunkFile(chunk, chunkNumber, comparer);
                            chunkFiles.Add(chunkFile);
                            yield return new ExternalChunkCreationStep(chunkFile, chunk.Count, chunkNumber);
                            chunkNumber++;
                            chunk.Clear();
                        }
                    }

                    // Handle the last chunk if it exists
                    if (chunk.Count > 0)
                    {
                        var chunkFile = CreateSortedChunkFile(chunk, chunkNumber, comparer);
                        chunkFiles.Add(chunkFile);
                        yield return new ExternalChunkCreationStep(chunkFile, chunk.Count, chunkNumber);
                    }
                }

                // Phase 2: Merge chunks
                while (chunkFiles.Count > 1)
                {
                    yield return new ExternalMergePhaseStep(chunkFiles.Count);
                    var newChunks = new List<string>();
                    
                    for (int i = 0; i < chunkFiles.Count - 1; i += 2)
                    {
                        var mergedFile = CreateTempFile("merged_");
                        using (var file1 = new StreamReader(chunkFiles[i]))
                        using (var file2 = new StreamReader(chunkFiles[i + 1]))
                        using (var output = new StreamWriter(mergedFile))
                        {
                            string? line1 = file1.ReadLine();
                            string? line2 = file2.ReadLine();

                            while (line1 != null && line2 != null)
                            {
                                if (comparer.Compare(line1, line2) <= 0)
                                {
                                    output.WriteLine(line1);
                                    yield return new ExternalMergeOperationStep(chunkFiles[i], chunkFiles[i + 1], mergedFile, line1, true);
                                    line1 = file1.ReadLine();
                                }
                                else
                                {
                                    output.WriteLine(line2);
                                    yield return new ExternalMergeOperationStep(chunkFiles[i], chunkFiles[i + 1], mergedFile, line2, false);
                                    line2 = file2.ReadLine();
                                }
                            }

                            // Copy remaining elements
                            while (line1 != null)
                            {
                                output.WriteLine(line1);
                                yield return new ExternalMergeOperationStep(chunkFiles[i], chunkFiles[i + 1], mergedFile, line1, true);
                                line1 = file1.ReadLine();
                            }

                            while (line2 != null)
                            {
                                output.WriteLine(line2);
                                yield return new ExternalMergeOperationStep(chunkFiles[i], chunkFiles[i + 1], mergedFile, line2, false);
                                line2 = file2.ReadLine();
                            }
                        }

                        newChunks.Add(mergedFile);
                        
                        // Clean up the merged files
                        File.Delete(chunkFiles[i]);
                        File.Delete(chunkFiles[i + 1]);
                    }

                    // If there's an odd file out, add it to the next round
                    if (chunkFiles.Count % 2 == 1)
                    {
                        var lastFile = chunkFiles[chunkFiles.Count - 1];
                        newChunks.Add(lastFile);
                    }

                    chunkFiles = newChunks;
                }

                // Move the final merged file to the output location
                if (chunkFiles.Count == 1)
                {
                    if (File.Exists(outputFilePath))
                        File.Delete(outputFilePath);
                        
                    File.Move(chunkFiles[0], outputFilePath);
                }

                yield return new ExternalMergePhaseStep(0, true);
            }
            finally
            {
                CleanupTempFiles();
            }
        }

        private string CreateSortedChunkFile(List<string> chunk, int chunkNumber, IComparer<string> comparer)
        {
            chunk.Sort(comparer);
            var chunkFile = CreateTempFile($"chunk_{chunkNumber}_");
            File.WriteAllLines(chunkFile, chunk);
            return chunkFile;
        }
    }
}
