using System.Windows.Controls;
using System.Windows.Media;
using WpfApp.Models.Steps;
using System.Windows.Shapes;
using System.Windows;
using System.Threading.Tasks;
using System;

namespace WpfApp.Services.Visualization
{
    public class InsertionSortVisualization : BaseVisualizationService
    {
        private readonly SolidColorBrush sortedPortionColor = VisualizationPalette.SortedPortionColor;

        private int lastSortedIndex = 0;
        public InsertionSortVisualization(Canvas visualizationCanvas, ListBox logList)
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
        }
    }
}
