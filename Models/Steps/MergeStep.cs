using System;
using System.Linq;

namespace WpfApp.Models.Steps
{
    public class MergeStep<T> : SortingStep<T>
    {
        public int LeftStart { get; }
        public int RightStart { get; }
        public int Length { get; }
        public bool IsMergeComplete { get; }

        public MergeStep(T[] array, int leftStart, int rightStart, int length, bool isMergeComplete = false, int delayMilliseconds = 500)
            : base(array, delayMilliseconds)
        {
            LeftStart = leftStart;
            RightStart = rightStart;
            Length = length;
            IsMergeComplete = isMergeComplete;
        }

        public override string GetDescription()
        {
            if (IsMergeComplete)
                return $"Merged subarrays: [{LeftStart}..{RightStart - 1}] and [{RightStart}..{LeftStart + Length - 1}]";
            return $"Merging subarrays: [{LeftStart}..{RightStart - 1}] and [{RightStart}..{LeftStart + Length - 1}]";
        }
    }
}