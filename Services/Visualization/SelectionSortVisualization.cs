using System;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using WpfApp.Models.Steps;

namespace WpfApp.Services.Visualization
{
    public class SelectionSortVisualization : BaseVisualizationService
    {
        private readonly SolidColorBrush minValueColor = VisualizationPalette.MinValueColor;
        private readonly SolidColorBrush sortedPortionColor = VisualizationPalette.SortedPortionColor;

        private int lastSortedIndex = 0;

        public SelectionSortVisualization(Canvas visualizationCanvas, ListBox logList)
            : base(visualizationCanvas, logList)
        {
        }

        public override void PreVisualizeStep(SortingStep<double> step)
        {
            if (step is not ArraySortingStep<double>) return;
            var arrayStep = (ArraySortingStep<double>)step;

            lastSortedIndex = arrayStep is LastSortedElementStep<double> moveStep ? moveStep.Index : lastSortedIndex;

            for (int i = 0; i <= Math.Min(arrayStep.CurrentArray.Length - 1, lastSortedIndex + 1); i++)
            {
                var rectangles = GetRectangles();
                rectangles[i].Fill = sortedPortionColor;
            }
            return;
        }

        public override void VisualizeAlgorithmSpecific(SortingStep<double> step)
        {
            if (step is CompareStep<double> compareStep)
            {
                // Color the current minimum value in purple
                var rectangles = GetRectangles();
                rectangles[compareStep.SecondIndex].Fill = minValueColor;

                // Add additional information to log
                LogMessage($"Current minimum value: {compareStep.CurrentArray[compareStep.SecondIndex]}");
            }
        }
    }
}
