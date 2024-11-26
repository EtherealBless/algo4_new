using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using WpfApp.Models.Steps;

namespace WpfApp.Services.Visualization
{
    public abstract class BaseVisualizationService : IVisualizationService
    {
        protected readonly Canvas visualizationCanvas;
        protected readonly ListBox logList;
        protected readonly SolidColorBrush defaultColor = VisualizationPalette.DefaultColor;
        protected readonly SolidColorBrush compareColor = VisualizationPalette.CompareColor;
        protected readonly SolidColorBrush swapColor = VisualizationPalette.SwapColor;

        protected BaseVisualizationService(Canvas visualizationCanvas, ListBox logList)
        {
            this.visualizationCanvas = visualizationCanvas;
            this.logList = logList;
        }

        public virtual void DrawArray(double[] array)
        {
            visualizationCanvas.Children.Clear();
            if (array == null || array.Length == 0) return;

            double maxValue = array.Max();
            double minValue = array.Min();
            double range = maxValue - minValue;
            double height = visualizationCanvas.ActualHeight / array.Length;
            double widthScale = visualizationCanvas.ActualWidth / range;  // Using full canvas width
            double minWidth = 2;  // Reduced minimum width for better proportions

            for (int i = 0; i < array.Length; i++)
            {
                double width = Math.Max(minWidth, (array[i] - minValue) * widthScale);
                var rect = new Rectangle
                {
                    Width = width,
                    Height = System.Math.Max(1, height - 1),
                    Fill = defaultColor
                };

                Canvas.SetLeft(rect, 0);
                Canvas.SetTop(rect, i * height);
                visualizationCanvas.Children.Add(rect);
            }
        }

        public virtual void PreVisualizeStep(SortingStep<double> step)
        {
            return;
        }

        public virtual async Task VisualizeStep(SortingStep<double> step)
        {


            var rectangles = GetRectangles();
            double[] array = step.CurrentArray;
            
            // Calculate scaling factors
            double maxValue = array.Max();
            double minValue = array.Min();
            double range = maxValue - minValue;
            double widthScale = visualizationCanvas.ActualWidth / range;

            // Update widths for all bars to match current array values
            Console.WriteLine($"Updating widths - Array state: {string.Join(", ", array)}");
            for (int i = 0; i < array.Length; i++)
            {
                double width = Math.Max(2, (array[i] - minValue) * widthScale);
                rectangles[i].Width = width;
                rectangles[i].Fill = defaultColor;
            }

            PreVisualizeStep(step);

            // Handle common step types
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

                case MoveStep<double> moveStep:
                    Console.WriteLine($"Visualizing move - From {moveStep.SourceIndex} to {moveStep.TargetIndex}");
                    Console.WriteLine($"Values - Source: {array[moveStep.SourceIndex]}, Target: {array[moveStep.TargetIndex]}");
                    rectangles[moveStep.SourceIndex].Fill = swapColor;
                    rectangles[moveStep.TargetIndex].Fill = swapColor;
                    break;
            }

            Console.WriteLine(step.GetDescription());
            Console.WriteLine(string.Join(", ", step.CurrentArray));

            // Handle algorithm-specific visualization
            VisualizeAlgorithmSpecific(step);

            await Task.Delay(step.DelayMilliseconds);
        }

        public abstract void VisualizeAlgorithmSpecific(SortingStep<double> step);

        public void LogMessage(string message)
        {
            logList.Items.Insert(0, $"[{DateTime.Now:HH:mm:ss}] {message}");
            while (logList.Items.Count > 100)
            {
                logList.Items.RemoveAt(logList.Items.Count - 1);
            }
        }

        public void ClearLog()
        {
            logList.Items.Clear();
        }

        protected List<Rectangle> GetRectangles()
        {
            return visualizationCanvas.Children.OfType<Rectangle>().ToList();
        }
    }
}
