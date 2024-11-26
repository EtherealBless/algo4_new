using System;
using System.Collections.Generic;
using WpfApp.Models.Steps;

namespace WpfApp.Algorithms
{
    public class QuickSort<T> : ISortingAlgorithm<T>
    {
        public string Name => "Quick Sort";
        public string Description => "Divides array into smaller subarrays using a pivot element.";
        public string TimeComplexity => "O(n log n)";

        private IComparer<T> comparer = Comparer<T>.Default;

        public IEnumerable<SortingStep<T>> Sort(T[] array, IComparer<T>? comparer = null)
        {
            this.comparer = comparer ?? Comparer<T>.Default;
            var allSteps = new List<SortingStep<T>>();

            // Add initial step
            var startStep = new AlgorithmStatusStep<T>(array, false, "Starting Quick Sort")
            {
                StepIndex = allSteps.Count,
                AllSteps = allSteps
            };
            allSteps.Add(startStep);
            yield return startStep;

            // Add quicksort steps
            foreach (var step in QuickSortInternal(array, 0, array.Length - 1))
            {
                step.StepIndex = allSteps.Count;
                step.AllSteps = allSteps;
                allSteps.Add(step);
                yield return step;
            }

            // Add final step
            var endStep = new AlgorithmStatusStep<T>(array, true, "Quick Sort Complete")
            {
                StepIndex = allSteps.Count,
                AllSteps = allSteps
            };
            allSteps.Add(endStep);
            yield return endStep;
        }

        private IEnumerable<SortingStep<T>> QuickSortInternal(T[] array, int low, int high)
        {
            if (low < high)
            {
                yield return new RecursionStep<T>(array, low, high, true);

                yield return new AlgorithmStatusStep<T>(array, false, $"Partitioning subarray [{low}..{high}]");

                int partitionIndex = -1;
                foreach (var step in Partition(array, low, high))
                {
                    if (step is PartitionStep<T> partitionStep)
                    {
                        partitionIndex = partitionStep.PivotIndex;
                    }
                    yield return step;
                }

                yield return new AlgorithmStatusStep<T>(array, false, $"Partition complete for [{low}..{high}]");

                if (partitionIndex > low)
                {
                    foreach (var step in QuickSortInternal(array, low, partitionIndex - 1))
                    {
                        yield return step;
                    }
                }

                if (partitionIndex < high)
                {
                    foreach (var step in QuickSortInternal(array, partitionIndex + 1, high))
                    {
                        yield return step;
                    }
                }

                yield return new RecursionStep<T>(array, low, high, false);
            }
        }

        private IEnumerable<SortingStep<T>> Partition(T[] array, int low, int high)
        {
            // Choose rightmost element as pivot
            int pivotIndex = high;
            T pivotValue = array[pivotIndex];

            // Show initial pivot state
            yield return new PartitionStep<T>(array, low, high, pivotIndex, pivotValue);

            int i = low - 1;

            for (int j = low; j < high; j++)
            {
                // Compare current element with pivot
                bool compareResult = comparer.Compare(array[j], pivotValue) <= 0;
                yield return new CompareStep<T>(array, j, pivotIndex, compareResult);

                if (compareResult)
                {
                    i++;
                    if (i != j)
                    {
                        // Show the pair before swapping
                        yield return new CompareStep<T>(array, i, j, true);

                        (array[j], array[i]) = (array[i], array[j]);
                        yield return new SwapStep<T>(array, i, j);

                        // Show partition state after swap
                        yield return new PartitionStep<T>(array, low, high, pivotIndex, pivotValue);
                    }
                }
            }

            // Final pivot swap
            if (i + 1 != pivotIndex)
            {
                // Show the pair before final pivot swap
                yield return new CompareStep<T>(array, i + 1, pivotIndex, true);
                (array[pivotIndex], array[i + 1]) = (array[i + 1], array[pivotIndex]);
                yield return new SwapStep<T>(array, i + 1, pivotIndex);
                // Show final partition state with pivot in its correct position
                pivotIndex = i + 1;
                yield return new PartitionStep<T>(array, low, high, pivotIndex, array[pivotIndex]);
            }
        }
    }
}
