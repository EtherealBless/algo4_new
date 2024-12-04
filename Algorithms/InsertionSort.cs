using System;
using System.Collections.Generic;
using WpfApp.Models.Steps;

namespace WpfApp.Algorithms
{
    public class InsertionSort<T> : ISortingAlgorithm<T>
    {
        public string Name => "Insertion Sort";
        public string Description => "Builds the sorted array by repeatedly inserting elements into their correct position.";
        public string TimeComplexity => "O(n²)";

        public IEnumerable<SortingStep<T>> Sort(T[] array, IComparer<T>? comparer = null)
        {

            comparer ??= Comparer<T>.Default;

            yield return new ArrayAlgorithmStatusStep<T>(array, false);

            for (int i = 1; i < array.Length; i++)
            {
                for (int j = i; j > 0; j--)
                {
                    int compareResult = comparer.Compare(array[j], array[j - 1]);
                    yield return new CompareStep<T>(array, j, j - 1, compareResult < 0);

                    if (compareResult < 0)
                    {
                        (array[j - 1], array[j]) = (array[j], array[j - 1]);
                        yield return new SwapStep<T>(array, j, j - 1);
                    }
                    else
                    {

                        yield return new LastSortedElementStep<T>(array, 0, i);
                        break;
                    }
                }
            }

            yield return new ArrayAlgorithmStatusStep<T>(array, true);
        }
    }
}
