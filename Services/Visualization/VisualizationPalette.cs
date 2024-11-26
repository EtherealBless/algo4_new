using System.Windows.Media;

namespace WpfApp.Services.Visualization
{
    public static class VisualizationPalette
    {
        // Base colors for array elements
        public static readonly SolidColorBrush DefaultColor = Brushes.Blue;
        
        // Colors for comparison and movement operations
        public static readonly SolidColorBrush CompareColor = Brushes.Red;
        public static readonly SolidColorBrush SwapColor = Brushes.Green;
        
        // Colors for sorted portions
        public static readonly SolidColorBrush SortedPortionColor = Brushes.LightGreen;
        
        // Special highlight colors
        public static readonly SolidColorBrush CurrentElementColor = Brushes.Yellow;
        public static readonly SolidColorBrush MinValueColor = Brushes.Purple;
        public static readonly SolidColorBrush PivotColor = Brushes.Orange;
        
        // QuickSort specific colors
        public static readonly SolidColorBrush GreaterThanPivotColor = Brushes.Pink;
    }
}
