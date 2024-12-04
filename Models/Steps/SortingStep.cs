using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace WpfApp.Models.Steps
{
    public abstract class ArraySortingStep<T> : SortingStep<T>
    {
        public T[] CurrentArray { get; }

        protected ArraySortingStep(T[] array, int delayMilliseconds = 500, string? statusMessage = null)
        : base(delayMilliseconds, statusMessage)
        {
            CurrentArray = array.ToArray();  // Make a copy of the array
        }

        // Method to describe what this step does (for UI/logging)
        public virtual string GetDescription()
        {
            return StatusMessage ?? "Unknown step";
        }
    }

    // Basic comparison step
    public class CompareStep<T> : ArraySortingStep<T>
    {
        public int FirstIndex { get; }
        public int SecondIndex { get; }
        public bool CompareResult { get; }

        public CompareStep(T[] array, int firstIndex, int secondIndex, bool compareResult)
            : base(array)
        {
            FirstIndex = firstIndex;
            SecondIndex = secondIndex;
            CompareResult = compareResult;
        }

        public override string GetDescription() =>
            $"Comparing elements at positions {FirstIndex} and {SecondIndex} (Result: {CompareResult})";
    }

    // Swapping two elements
    public class SwapStep<T> : ArraySortingStep<T>
    {
        public int FirstIndex { get; }
        public int SecondIndex { get; }

        public SwapStep(T[] array, int firstIndex, int secondIndex)
            : base(array)
        {
            FirstIndex = firstIndex;
            SecondIndex = secondIndex;
        }

        public override string GetDescription() =>
            $"Swapping elements at positions {FirstIndex} and {SecondIndex}";
    }

    // Moving an element to a new position
    public class MoveStep<T> : ArraySortingStep<T>
    {
        public int SourceIndex { get; }
        public int TargetIndex { get; }

        public MoveStep(T[] array, int sourceIndex, int targetIndex)
            : base(array)
        {
            SourceIndex = sourceIndex;
            TargetIndex = targetIndex;
        }

        public override string GetDescription() =>
            $"Moving element from position {SourceIndex} to {TargetIndex}";
    }

    // Shell sort gap change
    public class GapStep<T> : ArraySortingStep<T>
    {
        public int NewGap { get; }

        public GapStep(T[] array, int newGap)
            : base(array)
        {
            NewGap = newGap;
        }

        public override string GetDescription() =>
            $"Changing gap size to {NewGap}";
    }

    public class AlgorithmStatusStep<T> : ArraySortingStep<T>
    {
        public string StatusMessage { get; }
        public bool IsComplete { get; }

        public AlgorithmStatusStep(bool isComplete, string? statusMessage = null) : base(new T[0])
        {
            IsComplete = isComplete;
            StatusMessage = statusMessage;
        }
    }


    // Algorithm status steps
    public class ArrayAlgorithmStatusStep<T> : ArraySortingStep<T>
    {
        public bool IsComplete { get; }
        public new string StatusMessage { get; }

        public ArrayAlgorithmStatusStep(T[] array, bool isComplete, string? statusMessage = null)
            : base(array)
        {
            IsComplete = isComplete;
            StatusMessage = statusMessage ?? (isComplete ? "Sorting complete" : "Starting sort");
        }

        public override string GetDescription() => StatusMessage;
    }


    public class SortingStep<T>
    {
        public int DelayMilliseconds { get; }
        public int StepIndex { get; set; }  // Position in sequence
        public List<SortingStep<T>> AllSteps { get; set; } = new();  // Reference to all steps
        public string? StatusMessage { get; }

        protected SortingStep(int delayMilliseconds = 500, string? statusMessage = null)
        {
            DelayMilliseconds = delayMilliseconds;
            StatusMessage = statusMessage;
        }

        // Method to describe what this step does (for UI/logging)
        public virtual string GetDescription()
        {
            return StatusMessage ?? "Unknown step";
        }
    }



}