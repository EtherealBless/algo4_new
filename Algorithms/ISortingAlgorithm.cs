using System;
using System.Collections.Generic;
using WpfApp.Models.Steps;

namespace WpfApp.Algorithms
{
    public interface ISortingAlgorithm<T>
    {
        string Name { get; }
        string Description { get; }
        string TimeComplexity { get; }
        
        // Main sorting method that yields steps for visualization
        IEnumerable<SortingStep<T>> Sort(T[] array, IComparer<T>? comparer = null);
    }
}
