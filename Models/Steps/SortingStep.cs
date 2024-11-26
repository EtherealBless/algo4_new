using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace WpfApp.Models.Steps
{
    public abstract class SortingStep<T>
    {
        public T[] CurrentArray { get; }
        public int DelayMilliseconds { get; }
        public int StepIndex { get; set; }  // Position in sequence
        public List<SortingStep<T>> AllSteps { get; set; } = new();  // Reference to all steps
        public string? StatusMessage { get; }

        protected SortingStep(T[] array, int delayMilliseconds = 500, string? statusMessage = null)
        {
            CurrentArray = array.ToArray();  // Make a copy of the array
            DelayMilliseconds = delayMilliseconds;
            StatusMessage = statusMessage;
        }

        // Method to describe what this step does (for UI/logging)
        public virtual string GetDescription()
        {
            return StatusMessage ?? "Unknown step";
        }
    }

    // Basic comparison step
    public class CompareStep<T> : SortingStep<T>
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
    public class SwapStep<T> : SortingStep<T>
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
    public class MoveStep<T> : SortingStep<T>
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
    public class GapStep<T> : SortingStep<T>
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


    // Algorithm status steps
    public class AlgorithmStatusStep<T> : SortingStep<T>
    {
        public bool IsComplete { get; }
        public new string StatusMessage { get; }

        public AlgorithmStatusStep(T[] array, bool isComplete, string? statusMessage = null) 
            : base(array)
        {
            IsComplete = isComplete;
            StatusMessage = statusMessage ?? (isComplete ? "Sorting complete" : "Starting sort");
        }

        public override string GetDescription() => StatusMessage;
    }
}
