using System;
using System.Linq;
using System.Collections.Generic;
using WpfApp.Models.Steps;

namespace WpfApp.Algorithms
{
    public class NaturalMergeSort<T> : MergeSortBase<T>
    {
        public override string Name => "Natural Merge Sort";
        public override string Description => "A merge sort variant that takes advantage of existing ordered sequences in the input.";

        public override IEnumerable<SortingStep<T>> Sort(T[] array, IComparer<T>? comparer = null)
        {
            comparer ??= Comparer<T>.Default;
            T[] temp = new T[array.Length];
            Array.Copy(array, temp, array.Length);

            yield return new ArrayAlgorithmStatusStep<T>(array.ToArray(), false, "Starting Natural Merge Sort");

            bool isSorted = false;
            while (!isSorted)
            {
                isSorted = true;
                int left = 0;

                while (left < array.Length)
                {
                    // Find naturally sorted sequence
                    int right = left;
                    while (right + 1 < array.Length && comparer.Compare(array[right], array[right + 1]) <= 0)
                        right++;

                    // If we're at the end with just one sequence, and it started from the beginning
                    if (right == array.Length - 1 && left == 0)
                        break;

                    // Find next naturally sorted sequence
                    int nextLeft = right + 1;
                    if (nextLeft < array.Length)
                    {
                        int nextRight = nextLeft;
                        while (nextRight + 1 < array.Length && comparer.Compare(array[nextRight], array[nextRight + 1]) <= 0)
                            nextRight++;

                        // Show the two sequences we found
                        yield return new NaturalSequenceStep<T>(array.ToArray(), left, right, nextLeft, nextRight);

                        // Merge the two sequences
                        foreach (var step in Merge(array, temp, left, nextLeft, nextRight, comparer))
                            yield return step;

                        isSorted = false;
                        left = nextRight + 1;
                    }
                    else
                    {
                        left = array.Length;
                    }
                }
            }

            yield return new ArrayAlgorithmStatusStep<T>(array.ToArray(), true, "Natural Merge Sort completed");
        }
    }
}