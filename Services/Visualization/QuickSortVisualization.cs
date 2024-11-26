using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using System.Threading.Tasks;
using System.Collections.Generic;
using WpfApp.Models.Steps;
using System;
using System.Windows.Shapes;

namespace WpfApp.Services.Visualization
{
    public class QuickSortVisualization : BaseVisualizationService
    {
        private const double INDENT_WIDTH = 20.0; // Width of each indentation level
        private readonly List<(int Low, int High, int Level)> activeRanges = new();
        private int currentRecursionLevel = 0;
        private bool isFirstDraw = true;
        private int? lastPivotIndex = null;

        private readonly SolidColorBrush pivotColor = VisualizationPalette.PivotColor;
        private readonly SolidColorBrush lessThanPivotColor = VisualizationPalette.SortedPortionColor;
        private readonly SolidColorBrush greaterThanPivotColor = VisualizationPalette.GreaterThanPivotColor;

        public QuickSortVisualization(Canvas visualizationCanvas, ListBox logList)
            : base(visualizationCanvas, logList)
        {
        }

        private int GetIndentationLevel(int index)
        {
            var (_, _, Level) = activeRanges.FindLast(r => index >= r.Low && index <= r.High);
            return Level;
        }

        public override async Task VisualizeStep(SortingStep<double> step)
        {
            // Reset state if this is a backwards step (array has changed)
            if (!isFirstDraw && step.CurrentArray[0] != GetRectangles()[0].Width)
            {
                activeRanges.Clear();
                currentRecursionLevel = 0;
                lastPivotIndex = null;
                base.DrawArray(step.CurrentArray);

                // Rebuild state by replaying all steps up to this point
                for (int i = 0; i < step.StepIndex; i++)
                {
                    if (step.AllSteps[i] is RecursionStep<double> recursionStep)
                    {
                        if (recursionStep.IsEntering)
                        {
                            currentRecursionLevel++;
                            activeRanges.Add((recursionStep.Low, recursionStep.High, currentRecursionLevel));
                        }
                        else
                        {
                            activeRanges.RemoveAll(r => r.Low == recursionStep.Low && r.High == recursionStep.High);
                            currentRecursionLevel = Math.Max(0, currentRecursionLevel - 1);
                        }
                    }
                }

                // Update indentation for all elements
                var rectangles = GetRectangles();
                for (int i = 0; i < step.CurrentArray.Length; i++)
                {
                    Canvas.SetLeft(rectangles[i], GetIndentationLevel(i) * INDENT_WIDTH);
                }
            }
            else if (isFirstDraw)
            {
                base.DrawArray(step.CurrentArray);
                isFirstDraw = false;
            }
            else
            {

                // Update bar widths without redrawing
                var rectangles = GetRectangles();
                var array = step.CurrentArray;
                double maxValue = array.Max();
                double minValue = array.Min();
                double range = maxValue - minValue;
                double widthScale = visualizationCanvas.ActualWidth / range;

                for (int i = 0; i < array.Length; i++)
                {
                    double width = (array[i] - minValue) * widthScale;
                    rectangles[i].Width = width;
                    rectangles[i].Fill = defaultColor;
                }
            }

            VisualizeAlgorithmSpecific(step);
            await Task.Delay(step.DelayMilliseconds);
        }

        public override void VisualizeAlgorithmSpecific(SortingStep<double> step)
        {
            var rectangles = GetRectangles();

            if (step is PartitionStep<double> partitionStep)
            {
                lastPivotIndex = partitionStep.PivotIndex;

                // Highlight pivot
                rectangles[partitionStep.PivotIndex].Fill = pivotColor;

                // Color elements based on comparison with pivot
                double pivotValue = step.CurrentArray[partitionStep.PivotIndex];
                for (int i = partitionStep.Low; i <= partitionStep.High; i++)
                {
                    if (i != partitionStep.PivotIndex)
                    {
                        rectangles[i].Fill = step.CurrentArray[i] <= pivotValue
                            ? lessThanPivotColor  // Less than or equal to pivot
                            : greaterThanPivotColor;  // Greater than pivot
                    }
                }

                LogMessage($"Partitioning: pivot={pivotValue} at index {partitionStep.PivotIndex}");
            }
            else if (step is CompareStep<double> compareStep)
            {
                // Keep pivot colored
                if (lastPivotIndex.HasValue)
                {
                    rectangles[lastPivotIndex.Value].Fill = pivotColor;
                }

                // Color the elements being compared
                rectangles[compareStep.FirstIndex].Fill = compareColor;
                rectangles[compareStep.SecondIndex].Fill = compareColor;

                LogMessage($"Comparing elements at indices {compareStep.FirstIndex} and {compareStep.SecondIndex}");
            }
            else if (step is SwapStep<double> swapStep)
            {
                // Keep pivot colored
                if (lastPivotIndex.HasValue)
                {
                    rectangles[lastPivotIndex.Value].Fill = pivotColor;
                }

                // Color the elements being swapped
                rectangles[swapStep.FirstIndex].Fill = swapColor;
                rectangles[swapStep.SecondIndex].Fill = swapColor;

                LogMessage($"Swapping elements at indices {swapStep.FirstIndex} and {swapStep.SecondIndex}");
            }
            else if (step is RecursionStep<double> recursionStep)
            {

                if (recursionStep.IsEntering)
                {
                    currentRecursionLevel++;
                    activeRanges.Add((recursionStep.Low, recursionStep.High, currentRecursionLevel));
                    LogMessage($"Entering recursion level {currentRecursionLevel} for subarray [{recursionStep.Low}..{recursionStep.High}]");
                }
                else
                {
                    // Remove the current range
                    activeRanges.RemoveAll(r => r.Low == recursionStep.Low && r.High == recursionStep.High);
                    currentRecursionLevel = Math.Max(0, currentRecursionLevel - 1);
                    lastPivotIndex = null;  // Clear pivot reference when exiting recursion

                    // Reset colors for this subarray
                    for (int i = recursionStep.Low; i <= recursionStep.High; i++)
                    {
                        rectangles[i].Fill = defaultColor;
                    }
                    LogMessage($"Exiting recursion level {currentRecursionLevel + 1}");
                }

                // Update indentation for all elements based on their active ranges
                for (int i = recursionStep.Low; i <= recursionStep.High; i++)
                {
                    Canvas.SetLeft(rectangles[i], GetIndentationLevel(i) * INDENT_WIDTH);
                }
            }
        }

        public override void DrawArray(double[] array)
        {
            activeRanges.Clear();
            currentRecursionLevel = 0;
            isFirstDraw = true;
            base.DrawArray(array);
        }
    }
}
