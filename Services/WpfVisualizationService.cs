using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using WpfApp.Models.Steps;

namespace WpfApp.Services
{
    public class WpfVisualizationService : IVisualizationService<double>
    {
        private readonly Canvas visualizationCanvas;
        private readonly ListBox logList;
        private readonly SolidColorBrush defaultColor = Brushes.Blue;
        private readonly SolidColorBrush compareColor = Brushes.Red;
        private readonly SolidColorBrush swapColor = Brushes.Green;
        private readonly SolidColorBrush pivotColor = Brushes.Orange;

        public WpfVisualizationService(Canvas visualizationCanvas, ListBox logList)
        {
            this.visualizationCanvas = visualizationCanvas;
            this.logList = logList;
        }

        public void DrawArray(double[] array)
        {
            visualizationCanvas.Children.Clear();
            if (array == null || array.Length == 0) return;

            double maxValue = array.Max();
            double minValue = array.Min();
            double range = maxValue - minValue;
            double width = visualizationCanvas.ActualWidth / array.Length;
            double heightScale = visualizationCanvas.ActualHeight / range;

            for (int i = 0; i < array.Length; i++)
            {
                double height = (array[i] - minValue) * heightScale;
                var rect = new Rectangle
                {
                    Width = Math.Max(1, width - 1),
                    Height = height,
                    Fill = defaultColor
                };

                Canvas.SetLeft(rect, i * width);
                Canvas.SetBottom(rect, 0);
                visualizationCanvas.Children.Add(rect);
            }
        }

        public async Task VisualizeStep(SortingStep<double> step)
        {
            DrawArray(step.CurrentArray);
            var rectangles = visualizationCanvas.Children.OfType<Rectangle>().ToList();
            
            switch (step)
            {
                case CompareStep<double> compareStep:
                    rectangles[compareStep.FirstIndex].Fill = compareColor;
                    rectangles[compareStep.SecondIndex].Fill = compareColor;
                    break;

                case SwapStep<double> swapStep:
                    rectangles[swapStep.FirstIndex].Fill = swapColor;
                    rectangles[swapStep.SecondIndex].Fill = swapColor;
                    break;

                case PartitionStep<double> partitionStep:
                    rectangles[partitionStep.PivotIndex].Fill = pivotColor;
                    break;
            }

            LogMessage(step.GetDescription());
            await Task.Delay(step.DelayMilliseconds);
        }

        public void LogMessage(string message)
        {
            logList.Items.Insert(0, $"[{DateTime.Now:HH:mm:ss}] {message}");
            while (logList.Items.Count > 100) // Keep log size manageable
            {
                logList.Items.RemoveAt(logList.Items.Count - 1);
            }
        }

        public void ClearLog()
        {
            logList.Items.Clear();
        }
    }
}
