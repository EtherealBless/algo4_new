using System;
using System.Collections.Generic;
using System.IO;
using WpfApp.Models.Steps;

namespace WpfApp.Algorithms
{
    public abstract class ExternalMergeSortBase : IExternalSortingAlgorithm<string>
    {
        public abstract string Name { get; }
        public abstract string Description { get; }
        
        protected readonly string TempDirectory;
        
        protected ExternalMergeSortBase()
        {
            // Create temp directory in the same directory as input file
            TempDirectory = Path.Combine(Path.GetTempPath(), "ExternalMergeSort_" + Guid.NewGuid().ToString());
            Directory.CreateDirectory(TempDirectory);
        }

        public abstract IEnumerable<SortingStep<string>> Sort(string inputFilePath, string outputFilePath, IComparer<string> comparer);

        protected IEnumerable<SortingStep<string>> MergeTwoFiles(
            string file1Path, 
            string file2Path, 
            string outputPath, 
            IComparer<string> comparer)
        {
            using var file1 = new StreamReader(file1Path);
            using var file2 = new StreamReader(file2Path);
            using var output = new StreamWriter(outputPath);

            string? line1 = file1.ReadLine();
            string? line2 = file2.ReadLine();

            while (line1 != null && line2 != null)
            {
                if (comparer.Compare(line1, line2) <= 0)
                {
                    output.WriteLine(line1);
                    yield return new ExternalMergeCopyStep(line1, file1Path, outputPath);
                    line1 = file1.ReadLine();
                }
                else
                {
                    output.WriteLine(line2);
                    yield return new ExternalMergeCopyStep(line2, file2Path, outputPath);
                    line2 = file2.ReadLine();
                }
            }

            // Copy remaining elements from file1
            while (line1 != null)
            {
                output.WriteLine(line1);
                yield return new ExternalMergeCopyStep(line1, file1Path, outputPath);
                line1 = file1.ReadLine();
            }

            // Copy remaining elements from file2
            while (line2 != null)
            {
                output.WriteLine(line2);
                yield return new ExternalMergeCopyStep(line2, file2Path, outputPath);
                line2 = file2.ReadLine();
            }
        }

        protected string CreateTempFile(string prefix = "chunk_")
        {
            return Path.Combine(TempDirectory, $"{prefix}{Guid.NewGuid()}.txt");
        }

        protected void CleanupTempFiles()
        {
            if (Directory.Exists(TempDirectory))
            {
                Directory.Delete(TempDirectory, true);
            }
        }

        ~ExternalMergeSortBase()
        {
            CleanupTempFiles();
        }
    }
}
