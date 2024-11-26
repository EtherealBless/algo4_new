using System;
using System.Linq;
using System.Collections.Generic;
using WpfApp.Models.Steps;

namespace WpfApp.Algorithms
{
    public class ShellSort<T> : ISortingAlgorithm<T>
    {
        public string Name => "Shell Sort";
        public string Description => "Improves insertion sort by comparing elements far apart, then reducing the gap between elements to be compared.";
        public string TimeComplexity => "O(n log n) to O(n²)";

        private static IEnumerable<SortingStep<T>> ShiftElements(T[] array, T elementToInsert, int startIndex, int gap, IComparer<T> comparer)
        {
            int currentIndex = startIndex;
            List<int> positionsToShift = new();

            // First phase: Find all positions that need shifting
            while (currentIndex >= gap)
            {
                int compareIndex = currentIndex - gap;
                int compareResult = comparer.Compare(array[compareIndex], elementToInsert);
                yield return new CompareStep<T>(array, compareIndex, startIndex, compareResult > 0);

                if (compareResult <= 0)
                    break;

                positionsToShift.Add(currentIndex);
                currentIndex -= gap;
            }

            // Second phase: If shifts are needed, perform them all as one atomic operation
            if (positionsToShift.Count > 0)
            {
                // Shift elements up
                for (int i = 0; i < positionsToShift.Count; i++)
                {
                    int shiftTo = positionsToShift[i];
                    int shiftFrom = shiftTo - gap;
                    array[shiftTo] = array[shiftFrom];
                    yield return new MoveStep<T>(array.ToArray(), shiftFrom, shiftTo);
                }

                // Place the element in its final position
                array[currentIndex] = elementToInsert;
                yield return new MoveStep<T>(array.ToArray(), startIndex, currentIndex);
            }
        }

        public IEnumerable<SortingStep<T>> Sort(T[] array, IComparer<T>? comparer = null)
        {
            comparer ??= Comparer<T>.Default;
            yield return new AlgorithmStatusStep<T>(array, false);

            int gap = array.Length / 2;

            while (gap > 0)
            {
                yield return new GapStep<T>(array, gap);

                // Process each sub-array
                for (int i = gap; i < array.Length; i++)
                {
                    T elementToInsert = array[i];

                    // Handle the entire shifting operation as one atomic unit
                    foreach (var step in ShellSort<T>.ShiftElements(array, elementToInsert, i, gap, comparer))
                    {
                        yield return step;
                    }
                }

                gap /= 2;
            }

            yield return new AlgorithmStatusStep<T>(array, true);
        }
    }
}
