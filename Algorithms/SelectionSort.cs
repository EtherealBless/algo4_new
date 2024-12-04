using System;
using System.Collections.Generic;
using WpfApp.Models.Steps;

namespace WpfApp.Algorithms
{
    public class SelectionSort<T> : ISortingAlgorithm<T>
    {
        public string Name => "Selection Sort";
        public string Description => "Finds the minimum element in the unsorted portion and places it at the beginning.";
        public string TimeComplexity => "O(n²)";

        public IEnumerable<SortingStep<T>> Sort(T[] array, IComparer<T>? comparer = null)
        {
            comparer ??= Comparer<T>.Default;
            yield return new ArrayAlgorithmStatusStep<T>(array, false);

            for (int i = 0; i < array.Length - 1; i++)
            {
                int minIndex = i;
                
                // Find minimum element in unsorted portion
                for (int j = i + 1; j < array.Length; j++)
                {
                    int compareResult = comparer.Compare(array[j], array[minIndex]);
                    yield return new CompareStep<T>(array, j, minIndex, compareResult < 0);
                    
                    if (compareResult < 0)
                    {
                        minIndex = j;
                    }
                }

                // Swap if needed
                if (minIndex != i)
                {
                    yield return new SwapStep<T>(array, i, minIndex);
                    (array[i], array[minIndex]) = (array[minIndex], array[i]);
                }
                
                yield return new LastSortedElementStep<T>(array, 0, i);
            }

            yield return new ArrayAlgorithmStatusStep<T>(array, true);
        }
    }
}
