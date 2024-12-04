using System;
using System.IO;

namespace WpfApp.Models.Steps
{
    public class ExternalCompareStep<T> : SortingStep<T>
    {
        public T? FirstValue { get; }
        public T? SecondValue { get; }
        public string FirstFile { get; }
        public string SecondFile { get; }

        public int FirstIndex { get; }
        public int SecondIndex { get; }
        public bool CompareResult { get; }

        public ExternalCompareStep(T firstValue, T secondValue, string firstFile, string secondFile, int firstIndex, int secondIndex, bool compareResult)
            : base(500, $"Compare elements {firstValue} from {firstFile} and {secondValue} from {secondFile} on index {firstIndex} and {secondIndex} (Compare result: {compareResult})")
        {
            FirstValue = firstValue;
            SecondValue = secondValue;
            FirstFile = firstFile;
            SecondFile = secondFile;
            FirstIndex = firstIndex;
            SecondIndex = secondIndex;
            CompareResult = compareResult;
        }
    }

    public class ExternalMoveStep<T> : SortingStep<T>
    {
        public T Value { get; }
        public string SourceFile { get; }
        public string TargetFile { get; }
        public int SourceIndex { get; }
        public int TargetIndex { get; }

        public ExternalMoveStep(T value, string sourceFile, string targetFile, int sourceIndex, int targetIndex)
            : base(500, $"Move element {value} from file {sourceFile} to {targetFile} from index {sourceIndex} to {targetIndex}")
        {
            Value = value;
            SourceFile = sourceFile;
            TargetFile = targetFile;
            SourceIndex = sourceIndex;
            TargetIndex = targetIndex;
        }
    }

    public class ExternalChunkCreationStep : SortingStep<string>
    {
        public string FilePath { get; }
        public int ElementCount { get; }
        public int ChunkNumber { get; }

        public ExternalChunkCreationStep(string filePath, int elementCount, int chunkNumber) 
            : base(1000, $"Created chunk {chunkNumber} with {elementCount} elements")
        {
            FilePath = filePath;
            ElementCount = elementCount;
            ChunkNumber = chunkNumber;
        }
    }

    public class ExternalMergeOperationStep : SortingStep<string>
    {
        public string FirstFile { get; }
        public string SecondFile { get; }
        public string OutputFile { get; }
        public string CurrentElement { get; }
        public bool IsFromFirstFile { get; }

        public ExternalMergeOperationStep(string firstFile, string secondFile, string outputFile, string currentElement, bool isFromFirstFile)
            : base(500, $"Copying {currentElement} from {Path.GetFileName(isFromFirstFile ? firstFile : secondFile)} to {Path.GetFileName(outputFile)}")
        {
            FirstFile = firstFile;
            SecondFile = secondFile;
            OutputFile = outputFile;
            CurrentElement = currentElement;
            IsFromFirstFile = isFromFirstFile;
        }
    }

    public class ExternalMergePhaseStep : SortingStep<string>
    {
        public int RemainingChunks { get; }
        public bool IsComplete { get; }

        public ExternalMergePhaseStep(int remainingChunks, bool isComplete = false)
            : base(1500, isComplete ? "Merge sort completed" : $"Merging phase: {remainingChunks} chunks remaining")
        {
            RemainingChunks = remainingChunks;
            IsComplete = isComplete;
        }
    }
}