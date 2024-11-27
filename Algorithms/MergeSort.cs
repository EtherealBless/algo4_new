using System;
using System.Linq;
using System.Collections.Generic;
using WpfApp.Models.Steps;

namespace WpfApp.Algorithms
{
    public class MergeSort<T> : MergeSortBase<T>
    {
        public override string Name => "Merge Sort";
        public override string Description => "A divide-and-conquer algorithm that recursively splits the array into halves, sorts them, and merges them back together.";

        public override IEnumerable<SortingStep<T>> Sort(T[] array, IComparer<T>? comparer = null)
        {
            comparer ??= Comparer<T>.Default;
            
            // Show initial state
            yield return new AlgorithmStatusStep<T>(array.ToArray(), false, "Starting Merge Sort");
            
            // Create temporary array for merging
            T[] temp = new T[array.Length];
            Array.Copy(array, temp, array.Length);

            // Start recursive sorting
            foreach (var step in SortRecursive(array, temp, 0, array.Length - 1, comparer))
            {
                yield return step;
            }

            // Show completion
            yield return new AlgorithmStatusStep<T>(array.ToArray(), true, "Merge Sort completed");
        }

        private IEnumerable<SortingStep<T>> SortRecursive(T[] array, T[] temp, int start, int end, IComparer<T> comparer)
        {
            if (start >= end)
                yield break;

            int mid = (start + end) / 2;

            // Show recursive division
            yield return new RecursionStep<T>(array.ToArray(), start, end, true);

            // Sort left half
            foreach (var step in SortRecursive(array, temp, start, mid, comparer))
                yield return step;

            // Sort right half
            foreach (var step in SortRecursive(array, temp, mid + 1, end, comparer))
                yield return step;

            temp = new T[temp.Length];
            // Merge the sorted halves
            foreach (var step in Merge(array, temp, start, mid + 1, end, comparer))
                yield return step;

            // Show completion of this recursive level
            yield return new RecursionStep<T>(array.ToArray(), start, end, false);
        }
    }
}
