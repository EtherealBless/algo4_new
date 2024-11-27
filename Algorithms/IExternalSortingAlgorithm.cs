using System.Collections.Generic;
using WpfApp.Models.Steps;

namespace WpfApp.Algorithms
{
    public interface IExternalSortingAlgorithm<T>
    {
        string Name { get; }
        string Description { get; }
        IEnumerable<SortingStep<T>> Sort(string inputFilePath, string outputFilePath, IComparer<T> comparer);
    }
}