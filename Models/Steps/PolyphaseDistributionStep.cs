using System;
using System.Collections.Generic;

namespace WpfApp.Models.Steps
{
    public class PolyphaseDistributionStep<T> : ArraySortingStep<T>
    {
        public int SourceTapeNumber { get => sourceTapeNumber; }
        public int TargetTapeNumber { get => targetTapeNumber; }
        public string Value { get => value; }
        int sourceTapeNumber;
        int targetTapeNumber;
        string value;
        public PolyphaseDistributionStep(int sourceTapeNumber, int targetTapeNumber, string value) : base(new T[0], 0, $"Move {value} to tape {targetTapeNumber}")
        {
            this.sourceTapeNumber = sourceTapeNumber;
            this.targetTapeNumber = targetTapeNumber;
            this.value = value;
        }
    }
}
