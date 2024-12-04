using System;
using System.Linq;
using System.Collections.Generic;
using WpfApp.Models.Steps;

namespace WpfApp.Algorithms
{
    public abstract class MergeSortBase<T> : ISortingAlgorithm<T>
    {
        public abstract string Name { get; }
        public abstract string Description { get; }
        public virtual string TimeComplexity => "O(n log n)";

        public abstract IEnumerable<SortingStep<T>> Sort(T[] array, IComparer<T>? comparer = null);

        protected IEnumerable<SortingStep<T>> Merge(T[] array, T[] temp, int leftStart, int rightStart, int end, IComparer<T> comparer)
        {
            int leftEnd = rightStart - 1;
            int tempPos = leftStart;
            int originalLeftStart = leftStart;  // Store original positions
            int originalRightStart = rightStart;
            int length = end - leftStart + 1;
    
            // Show the subarrays that will be merged
            yield return new MergeStep<T>(array.ToArray(), leftStart, rightStart, length);

            while (leftStart <= leftEnd && rightStart <= end)
            {
                if (comparer.Compare(array[leftStart], array[rightStart]) <= 0)
                {
                    temp[tempPos] = array[leftStart];
                    yield return new MergeCopyStep<T>(array.ToArray(), leftStart, array[leftStart], tempPos, true);
                    tempPos++;
                    leftStart++;
                }
                else
                {
                    temp[tempPos] = array[rightStart];
                    yield return new MergeCopyStep<T>(array.ToArray(), rightStart, array[rightStart], tempPos, false);
                    tempPos++;
                    rightStart++;
                }
            }

            while (leftStart <= leftEnd)
            {
                temp[tempPos] = array[leftStart];
                yield return new MergeCopyStep<T>(array.ToArray(), leftStart, array[leftStart], tempPos, true);
                tempPos++;
                leftStart++;
            }

            while (rightStart <= end)
            {
                temp[tempPos] = array[rightStart];
                yield return new MergeCopyStep<T>(array.ToArray(), rightStart, array[rightStart], tempPos, false);
                tempPos++;
                rightStart++;
            }

            // Copy back from temp to array
            for (int i = 0; i < length; i++)
            {
                array[originalLeftStart + i] = temp[originalLeftStart + i];
            }

            // Show the completed merge using the original positions
            yield return new MergeStep<T>(array.ToArray(), originalLeftStart, originalRightStart, length, true);
        }
    }
}