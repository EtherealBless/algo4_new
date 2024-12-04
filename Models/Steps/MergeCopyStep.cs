using System;
using System.Linq;

namespace WpfApp.Models.Steps
{
    public class MergeCopyStep<T> : ArraySortingStep<T>
    {
        public int SourceIndex { get; }
        public T Value { get; }
        public int TargetIndex { get; }
        public bool IsFromLeftArray { get; }

        public MergeCopyStep(T[] array, int sourceIndex, T value, int targetIndex, bool isFromLeftArray, int delayMilliseconds = 500)
            : base(array, delayMilliseconds)
        {
            SourceIndex = sourceIndex;
            Value = value;
            TargetIndex = targetIndex;
            IsFromLeftArray = isFromLeftArray;
        }

        public override string GetDescription()
        {
            string arrayPart = IsFromLeftArray ? "left" : "right";
            return $"Copying element from {arrayPart} array at position {SourceIndex} to temporary array at position {TargetIndex}";
        }
    }
}
