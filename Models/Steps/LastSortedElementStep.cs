namespace WpfApp.Models.Steps
{
    public class LastSortedElementStep<T> : SortingStep<T>
    {
        public LastSortedElementStep(T[] array, int delayMilliseconds = 500, int index = 0) : base(array, delayMilliseconds)
        {
            Index = index;
        }

        public int Index { get; set; }
        public int Value { get; set; }

        public override string GetDescription()
        {
            return $"Inserting {Value} at index {Index}";
        }

        public int InsertIndex { get; set; }
    }
}