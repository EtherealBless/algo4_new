using System;

namespace WpfApp.Models.Steps
{
    public class PartitionStep<T> : ArraySortingStep<T>
    {
        public int Low { get; }
        public int High { get; }
        public int PivotIndex { get; }
        public T PivotValue { get; }

        public PartitionStep(T[] array, int low, int high, int pivotIndex, T pivotValue) 
            : base(array)
        {
            Low = low;
            High = high;
            PivotIndex = pivotIndex;
            PivotValue = pivotValue;
        }

        public override string GetDescription()
        {
            return $"Partitioning [{Low}..{High}] with pivot {PivotValue} at index {PivotIndex}";
        }
    }
}
