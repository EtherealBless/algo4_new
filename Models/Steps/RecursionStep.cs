using System;

namespace WpfApp.Models.Steps
{
    public class RecursionStep<T> : SortingStep<T>
    {
        public int Low { get; }
        public int High { get; }
        public bool IsEntering { get; }

        public RecursionStep(T[] array, int low, int high, bool isEntering)
            : base(array)
        {
            Low = low;
            High = high;
            IsEntering = isEntering;
        }

        public override string GetDescription()
        {
            return IsEntering
                ? $"Entering subarray [{Low}..{High}]"
                : $"Exiting subarray [{Low}..{High}]";
        }
    }
}
