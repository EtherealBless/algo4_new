using System;

namespace WpfApp.Models.Steps
{
    public class NaturalSequenceStep<T> : ArraySortingStep<T>
    {
        public int FirstSequenceStart { get; }
        public int FirstSequenceEnd { get; }
        public int SecondSequenceStart { get; }
        public int SecondSequenceEnd { get; }

        public NaturalSequenceStep(T[] array, int firstStart, int firstEnd, int secondStart, int secondEnd) 
            : base(array)
        {
            FirstSequenceStart = firstStart;
            FirstSequenceEnd = firstEnd;
            SecondSequenceStart = secondStart;
            SecondSequenceEnd = secondEnd;
        }
    }
}
