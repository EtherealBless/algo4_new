using System.Windows.Controls;
using System.Windows.Media;
using WpfApp.Models.Steps;

namespace WpfApp.Services.Visualization
{
    public class ShellSortVisualization : BaseVisualizationService
    {
        private readonly SolidColorBrush[] subArrayColors = new[]
        {
            Brushes.LightPink,
            Brushes.LightGreen,
            Brushes.LightBlue,
            Brushes.LightYellow
        };

        private int currentGap = 1;

        public ShellSortVisualization(Canvas visualizationCanvas, ListBox logList) 
            : base(visualizationCanvas, logList)
        {
        }

        public override void VisualizeAlgorithmSpecific(SortingStep<double> step)
        {
            if (step is GapStep<double> gapStep)
            {
                currentGap = gapStep.NewGap;
                var rectangles = GetRectangles();

                // Color each sub-array with a different color
                for (int i = 0; i < gapStep.CurrentArray.Length; i++)
                {
                    int subArrayIndex = i % currentGap;
                    rectangles[i].Fill = subArrayColors[subArrayIndex % subArrayColors.Length];
                }

                LogMessage($"Gap changed to {currentGap}, Number of sub-arrays: {currentGap}");
            }
            else if (step is MoveStep<double> moveStep)
            {
                var rectangles = GetRectangles();
                rectangles[moveStep.SourceIndex].Fill = Brushes.Green;
                rectangles[moveStep.TargetIndex].Fill = Brushes.Green;

                LogMessage($"Moving element from position {moveStep.SourceIndex} to {moveStep.TargetIndex}");
            }
            else if (step is CompareStep<double> compareStep)
            {
                var rectangles = GetRectangles();
                rectangles[compareStep.FirstIndex].Fill = Brushes.Red;
                rectangles[compareStep.SecondIndex].Fill = Brushes.Red;

                LogMessage($"Comparing elements {currentGap} positions apart");
            }
        }
    }
}
