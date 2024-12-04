using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Collections.Generic;
using WpfApp.Models.Steps;

namespace WpfApp.Services.Visualization
{
    public partial class MergeSortVisualization : UserControl
    {
        private const double INDENT_HEIGHT = 15.0; // Height of each indentation level
        private readonly SolidColorBrush CompareColor = new(Colors.Red);
        private readonly SolidColorBrush MergeColor = new(Colors.Purple);
        private readonly SolidColorBrush DefaultColor = new(Colors.Blue);
        private readonly SolidColorBrush TempColor = new(Colors.Green);
        private readonly List<SolidColorBrush> NaturalSequenceColors = new()
        {
            new SolidColorBrush(Colors.Orange),
            new SolidColorBrush(Colors.YellowGreen),
            new SolidColorBrush(Colors.Pink),
            new SolidColorBrush(Colors.Cyan),
            new SolidColorBrush(Colors.Gold)
        };
        private int currentNaturalColorIndex = 0;
        private Rectangle[] mainArrayRects;
        private Rectangle[] tempArrayRects;
        private double[] currentArray;
        private double[] originalArray;
        private Dictionary<int, double> activeValues = new();
        private List<(int Start, int End, int Level)> activeRanges = new();
        private int currentRecursionLevel = 0;

        public MergeSortVisualization()
        {
            InitializeComponent();
            Loaded += MergeSortVisualization_Loaded;
            SizeChanged += UserControl_SizeChanged;
        }

        private void MergeSortVisualization_Loaded(object sender, RoutedEventArgs e)
        {
            if (currentArray != null)
            {
                UpdateCanvasLayout();
            }
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateCanvasLayout();
        }

        private void UpdateCanvasLayout()
        {
            if (mainArrayRects == null) return;

            double rectWidth = Math.Min(30, (MainArrayCanvas.ActualWidth / mainArrayRects.Length) - 4);

            // Update main array layout
            for (int i = 0; i < mainArrayRects.Length; i++)
            {
                mainArrayRects[i].Width = rectWidth;
                Canvas.SetLeft(mainArrayRects[i], i * (rectWidth + 4));
                Canvas.SetBottom(mainArrayRects[i], GetIndentationForIndex(i));
            }

            // Update temp array layout
            for (int i = 0; i < tempArrayRects.Length; i++)
            {
                tempArrayRects[i].Width = rectWidth;
                Canvas.SetLeft(tempArrayRects[i], i * (rectWidth + 4));
                Canvas.SetBottom(tempArrayRects[i], 0);
            }
        }

        public void InitializeArrays(double[] array)
        {
            originalArray = array.ToArray();
            activeValues.Clear();
            activeRanges.Clear();
            currentRecursionLevel = 0;

            MainArrayCanvas.Children.Clear();
            TempArrayCanvas.Children.Clear();

            // Create rectangles for main array
            mainArrayRects = new Rectangle[array.Length];
            double maxValue = array.Max();
            double minValue = array.Min();
            double range = maxValue - minValue;
            double rectWidth = Math.Min(30, (MainArrayCanvas.ActualWidth / array.Length) - 4);

            for (int i = 0; i < array.Length; i++)
            {
                double height = ((array[i] - minValue) / range) * (MainArrayCanvas.ActualHeight * 0.7);
                var rect = new Rectangle
                {
                    Width = rectWidth,
                    Height = Math.Max(height, 1),
                    Fill = DefaultColor,
                    Margin = new Thickness(2, 0, 2, 0)
                };
                mainArrayRects[i] = rect;
                MainArrayCanvas.Children.Add(rect);
                Canvas.SetLeft(rect, i * (rectWidth + 4));
                Canvas.SetBottom(rect, 0);
            }

            // Create rectangles for temp array (initially hidden)
            tempArrayRects = new Rectangle[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                var rect = new Rectangle
                {
                    Width = rectWidth,
                    Height = 0,  // Start with zero height
                    Fill = TempColor,
                    Opacity = 0,  // Start fully transparent
                    Margin = new Thickness(2, 0, 2, 0)
                };
                tempArrayRects[i] = rect;
                TempArrayCanvas.Children.Add(rect);
                Canvas.SetLeft(rect, i * (rectWidth + 4));
                Canvas.SetBottom(rect, 0);
            }
        }

        private void ApplyIndentation()
        {
            if (mainArrayRects == null) return;

            foreach (var rect in mainArrayRects)
            {
                int index = mainArrayRects.ToList().IndexOf(rect);
                int level = GetIndentationLevel(index);
                Canvas.SetBottom(rect, level * INDENT_HEIGHT);
            }
        }

        private int GetIndentationLevel(int index)
        {
            var range = activeRanges.FindLast(r => index >= r.Start && index <= r.End);
            return range.Level;
        }

        private double GetIndentationForIndex(int index)
        {
            var range = activeRanges.FindLast(r => index >= r.Start && index <= r.End);
            return range.Level * INDENT_HEIGHT;
        }

        public void UpdateVisualization(SortingStep<double> step)
        {
            if (step == null || step is not ArraySortingStep<double>) return;

            var arrayStep = (ArraySortingStep<double>)step;
            // Reset colors
            foreach (var rect in mainArrayRects)
                rect.Fill = DefaultColor;
            foreach (var rect in tempArrayRects)
                rect.Fill = DefaultColor;

            currentArray = arrayStep.CurrentArray.ToArray();

            if (step is MergeStep<double> mergeStep)
            {
                HandleMergeStep(mergeStep);
            }
            else if (step is MergeCopyStep<double> copyStep)
            {
                HandleMergeCopyStep(copyStep);
            }
            else if (step is RecursionStep<double> recursionStep)
            {
                HandleRecursionStep(recursionStep);
            }
            else if (step is NaturalSequenceStep<double> naturalStep)
            {
                HandleNaturalSequenceStep(naturalStep);
            }

            UpdateArrayValues();
        }

        private void HandleNaturalSequenceStep(NaturalSequenceStep<double> step)
        {
            // Get colors for this pair of sequences
            var firstColor = NaturalSequenceColors[currentNaturalColorIndex];
            var secondColor = NaturalSequenceColors[(currentNaturalColorIndex + 1) % NaturalSequenceColors.Count];

            // Highlight first sequence
            for (int i = step.FirstSequenceStart; i <= step.FirstSequenceEnd; i++)
            {
                mainArrayRects[i].Fill = firstColor;
            }

            // Highlight second sequence
            for (int i = step.SecondSequenceStart; i <= step.SecondSequenceEnd; i++)
            {
                mainArrayRects[i].Fill = secondColor;
            }

            // Update color index for next pair of sequences
            currentNaturalColorIndex = (currentNaturalColorIndex + 2) % NaturalSequenceColors.Count;

            // Add ranges for indentation
            activeRanges.Clear();
            activeRanges.Add((step.FirstSequenceStart, step.FirstSequenceEnd, 0));
            activeRanges.Add((step.SecondSequenceStart, step.SecondSequenceEnd, 0));
            ApplyIndentation();
        }

        private void HandleMergeStep(MergeStep<double> mergeStep)
        {
            // Clear temp array before starting new merge
            foreach (var rect in tempArrayRects)
            {
                rect.Height = 0;
                rect.Opacity = 0;
            }

            // Show the merging process in main array
            for (int i = mergeStep.LeftStart; i < mergeStep.LeftStart + mergeStep.Length; i++)
            {
                if (i < mergeStep.RightStart)
                {
                    mainArrayRects[i].Fill = CompareColor;
                }
                else
                {
                    mainArrayRects[i].Fill = MergeColor;
                }

                // Update heights based on current array values
                double value = mergeStep.CurrentArray[i];
                double height = ((value - originalArray.Min()) / (originalArray.Max() - originalArray.Min())) * (MainArrayCanvas.ActualHeight * 0.7);
                mainArrayRects[i].Height = Math.Max(height, 1);
            }

            if (mergeStep.IsMergeComplete)
            {
                // First update the main array with sorted values
                int tempLength = mergeStep.Length;
                for (int i = 0; i < tempLength; i++)
                {
                    int mainArrayIndex = mergeStep.LeftStart + i;
                    double value = mergeStep.CurrentArray[mainArrayIndex];
                    double height = ((value - originalArray.Min()) / (originalArray.Max() - originalArray.Min())) * (MainArrayCanvas.ActualHeight * 0.7);
                    mainArrayRects[mainArrayIndex].Height = Math.Max(height, 1);
                    mainArrayRects[mainArrayIndex].Fill = DefaultColor;
                }

                // Then clear the temp array
                for (int i = 0; i < tempLength; i++)
                {
                    tempArrayRects[i].Height = 0;
                    tempArrayRects[i].Opacity = 0;
                }
            }
            else
            {
                int tempLength = mergeStep.Length;

                for (int i = 0; i < tempLength; i++)
                {
                    tempArrayRects[i].Height = 0;
                    tempArrayRects[i].Opacity = 0;
                }
                // Show current state in temp array
                for (int i = mergeStep.LeftStart; i < mergeStep.RightStart; i++)
                {
                    int mainArrayIndex = mergeStep.LeftStart + i;
                    double value = mergeStep.CurrentArray[mainArrayIndex];
                    double height = (value - originalArray.Min()) / (originalArray.Max() - originalArray.Min()) * (MainArrayCanvas.ActualHeight * 0.7);

                    tempArrayRects[i].Height = Math.Max(height, 1);
                    tempArrayRects[i].Opacity = 1;
                    tempArrayRects[i].Fill = TempColor;
                }
            }
        }

        private void HandleMergeCopyStep(MergeCopyStep<double> copyStep)
        {
            // Highlight the source element in main array
            mainArrayRects[copyStep.SourceIndex].Fill = CompareColor;

            // Show the element being copied in temp array
            double value = copyStep.CurrentArray[copyStep.SourceIndex];
            double height = ((value - originalArray.Min()) / (originalArray.Max() - originalArray.Min())) * (MainArrayCanvas.ActualHeight * 0.7);

            // Calculate temp array index relative to the current merge operation
            int tempIndex = copyStep.TargetIndex;
            tempArrayRects[tempIndex].Height = Math.Max(height, 1);
            tempArrayRects[tempIndex].Opacity = 1;
            tempArrayRects[tempIndex].Fill = copyStep.IsFromLeftArray ? CompareColor : MergeColor;

            // Update active values and main array height
            activeValues[copyStep.SourceIndex] = copyStep.CurrentArray[copyStep.SourceIndex];
            mainArrayRects[copyStep.SourceIndex].Height = Math.Max(height, 1);
        }

        private void HandleRecursionStep(RecursionStep<double> recursionStep)
        {
            // Update recursion tracking
            if (recursionStep.IsEntering)
            {
                currentRecursionLevel++;
                activeRanges.Add((recursionStep.Low, recursionStep.High, currentRecursionLevel));

                // Reset active values and temp array for this range
                for (int i = recursionStep.Low; i <= recursionStep.High; i++)
                {
                    activeValues[i] = originalArray[i];
                }
            }
            else
            {
                activeRanges.RemoveAll(r => r.Start == recursionStep.Low && r.End == recursionStep.High);
                currentRecursionLevel = Math.Max(0, currentRecursionLevel - 1);

                // Update original array with sorted values
                for (int i = recursionStep.Low; i <= recursionStep.High; i++)
                {
                    originalArray[i] = recursionStep.CurrentArray[i];
                    activeValues.Remove(i);
                }
            }
            UpdateCanvasLayout();
        }

        private void UpdateArrayValues()
        {
            double maxValue = originalArray.Max();
            double minValue = originalArray.Min();
            double range = maxValue - minValue;

            // Update heights for active ranges only
            foreach (var (start, end, _) in activeRanges)
            {
                for (int i = start; i <= end; i++)
                {
                    activeValues[i] = currentArray[i];
                    double value = currentArray[i];
                    double height = ((value - minValue) / range) * (MainArrayCanvas.ActualHeight * 0.7);
                    mainArrayRects[i].Height = Math.Max(height, 1);
                }
            }

            ApplyIndentation();
        }

        private Rectangle CreateRectangle(double value, double maxValue, double minValue, double width, int index, double canvasHeight)
        {
            // Calculate height based on full canvas height, independent of recursion level
            double height = ((value - minValue) / (maxValue - minValue)) * (canvasHeight * 0.7);
            var rect = new Rectangle
            {
                Width = width * 0.9,
                Height = Math.Max(height, 1),
                Fill = DefaultColor
            };

            Canvas.SetLeft(rect, index * width);
            Canvas.SetBottom(rect, 0);
            return rect;
        }

        private Rectangle CreateRectangle(double height)
        {
            var rect = new Rectangle
            {
                Width = 30,  // Set a fixed width for the rectangle
                Height = Math.Max(height, 1),
                Fill = DefaultColor,
                Margin = new Thickness(2, 0, 2, 0)  // Add some margin between rectangles
            };
            return rect;
        }
    }
}