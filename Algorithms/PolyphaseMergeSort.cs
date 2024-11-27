using System;
using System.Collections.Generic;
using WpfApp.Models.Steps;

namespace WpfApp.Algorithms
{
    public class PolyphaseMergeSort<T> : MergeSortBase<T>
    {
        public override string Name => "Polyphase Merge Sort";
        public override string Description => "A merge sort variant designed for external sorting that distributes runs across multiple tapes.";

        public override IEnumerable<SortingStep<T>> Sort(T[] array, IComparer<T>? comparer = null)
        {
            // Implementation will be added later
            yield break;
        }
    }
}