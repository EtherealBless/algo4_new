using System;
using System.Collections.Generic;
using WpfApp.Models.Steps;

namespace WpfApp.Algorithms
{
    public class RadixSort : ISortingAlgorithm<int>
    {
        public string Name => "Radix Sort";
        public string Description => "Sorts integers by processing each digit position, starting from the least significant digit.";
        public string TimeComplexity => "O(d * n) where d is the number of digits";

        public IEnumerable<SortingStep<int>> Sort(int[] array, IComparer<int>? comparer = null)
        {
            yield return new ArrayAlgorithmStatusStep<int>(array, false);

            if (array.Length <= 1)
            {
                yield return new ArrayAlgorithmStatusStep<int>(array, true);
                yield break;
            }

            int max = array[0];
            for (int i = 1; i < array.Length; i++)
            {
                if (array[i] > max)
                {
                    max = array[i];
                    yield return new CompareStep<int>(array, i, 0, true);
                }
                else
                {
                    yield return new CompareStep<int>(array, i, 0, false);
                }
            }

            for (int exp = 1; max / exp > 0; exp *= 10)
            {
                int[] output = new int[array.Length];
                int[] count = new int[10];

                // Store count of occurrences
                for (int i = 0; i < array.Length; i++)
                {
                    count[(array[i] / exp) % 10]++;
                }

                // Change count[i] to contain actual position
                for (int i = 1; i < 10; i++)
                {
                    count[i] += count[i - 1];
                }

                // Build the output array
                for (int i = array.Length - 1; i >= 0; i--)
                {
                    int digit = (array[i] / exp) % 10;
                    output[count[digit] - 1] = array[i];
                    count[digit]--;
                }

                // Copy the output array to array
                for (int i = 0; i < array.Length; i++)
                {
                    array[i] = output[i];
                    yield return new SwapStep<int>(array, i, i);
                }

                yield return new LastSortedElementStep<int>(array, 0, array.Length - 1);
            }

            yield return new ArrayAlgorithmStatusStep<int>(array, true);
        }
    }
}