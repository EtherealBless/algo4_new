using System;
using System.Linq;
using System.Collections.Generic;

namespace WpfApp.Models.Steps
{
    public class PolyphaseMergeStep<T> : SortingStep<T>
    {
        public int InputTape { get; }
        public int InputIndex { get; }
        public int OutputTape { get; }
        public int OutputIndex { get; }
        public string Value { get; }
        public bool IsMergeComplete { get; }

        public PolyphaseMergeStep(int inputTape, int outputTape, int inputIndex, int outputIndex, string value) : base(new T[0], 0)
        {
            InputTape = inputTape;
            OutputTape = outputTape;
            InputIndex = inputIndex;
            OutputIndex = outputIndex;
            Value = value;
        }


        public override string GetDescription()
        {

            return $"Write {Value} from [{InputTape}][{InputIndex}] to [{OutputTape}][{OutputIndex}]";
        }
    }
}
