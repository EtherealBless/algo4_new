using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using WpfApp.Models.Steps;

namespace WpfApp.Services.Visualization
{
    public partial class PolyphaseMergeSortVisualization : UserControl
    {
        private const double BAR_MARGIN = 2;
        private const double MIN_BAR_HEIGHT = 20;
        private const double MAX_BAR_HEIGHT = 100;
        private const double MIN_BAR_WIDTH = 20;
        private const double MAX_BAR_WIDTH = 80;

        private readonly SolidColorBrush DistributionColor = new(Colors.Blue);
        private readonly SolidColorBrush MergeColor = new(Colors.Green);
        private readonly SolidColorBrush DefaultColor = new(Colors.Gray);
        private readonly SolidColorBrush HighlightColor = new(Colors.Red);
        private readonly SolidColorBrush TextColor = new(Colors.Black);

        private int numberOfTapes;
        private List<List<(Rectangle Bar, TextBlock Label, double Value)>> tapeElements;
        private Canvas[] tapeCanvases;

        private Dictionary<string, double> valueMapping = new Dictionary<string, double>();
        private int nextValue = 0;

        private double GetNumericValue(string value)
        {
            if (!valueMapping.ContainsKey(value))
            {
                valueMapping[value] = nextValue++;
            }
            return valueMapping[value];
        }

        public PolyphaseMergeSortVisualization()
        {
            InitializeComponent();
            tapeElements = new List<List<(Rectangle Bar, TextBlock Label, double Value)>>();
            this.Loaded += PolyphaseMergeSortVisualization_Loaded;
        }

        private void PolyphaseMergeSortVisualization_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateLayout();
        }

        public void Initialize(int tapes)
        {
            numberOfTapes = tapes;
            tapeElements.Clear();

            // Clear existing canvases
            TapesContainer.Children.Clear();
            TapesContainer.ColumnDefinitions.Clear();

            // Create column definitions
            for (int i = 0; i < numberOfTapes; i++)
            {
                TapesContainer.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            }

            // Create canvases for each tape
            tapeCanvases = new Canvas[numberOfTapes];
            for (int i = 0; i < numberOfTapes; i++)
            {
                var canvas = new Canvas
                {
                    Background = new SolidColorBrush(Colors.WhiteSmoke),
                    Margin = new Thickness(5),
                    MinHeight = MAX_BAR_HEIGHT + 2 * BAR_MARGIN
                };
                Grid.SetColumn(canvas, i);
                TapesContainer.Children.Add(canvas);
                tapeCanvases[i] = canvas;
                tapeElements.Add(new List<(Rectangle Bar, TextBlock Label, double Value)>());

                // Add label
                var label = new TextBlock
                {
                    Text = $"Tape {i + 1}",
                    Margin = new Thickness(5, 0, 0, 0),
                    VerticalAlignment = VerticalAlignment.Top
                };
                canvas.Children.Add(label);
            }

            StatusText.Text = $"Initialized {numberOfTapes} tapes";
            UpdateLayout();
        }

        public void DrawInitialArray(IEnumerable<string> values)
        {
            Dispatcher.Invoke(() =>
            {
                if (tapeCanvases == null || tapeCanvases.Length == 0)
                {
                    StatusText.Text = "Error: Tapes not initialized";
                    return;
                }

                var valuesList = values.ToList();

                // Clear first tape
                tapeElements[0].Clear();
                tapeCanvases[0].Children.Clear();

                // Add tape label back
                var tapeLabel = new TextBlock
                {
                    Text = "Tape 1",
                    Margin = new Thickness(5, 0, 0, 0),
                    VerticalAlignment = VerticalAlignment.Top
                };
                tapeCanvases[0].Children.Add(tapeLabel);

                // Calculate bar width based on number of elements and canvas width
                double availableWidth = tapeCanvases[0].ActualWidth - 2 * BAR_MARGIN;
                double barWidth = Math.Max(MIN_BAR_WIDTH, Math.Min(MAX_BAR_WIDTH, 
                    (availableWidth - BAR_MARGIN * (valuesList.Count - 1)) / valuesList.Count));

                // Sort values to determine min and max for height calculation
                var sortedValues = valuesList.OrderBy(s => s, StringComparer.Ordinal).ToList();
                int totalValues = sortedValues.Count;
                
                double x = BAR_MARGIN;
                foreach (var value in valuesList)
                {
                    // Calculate height based on lexicographical position
                    int position = sortedValues.IndexOf(value);
                    double normalizedHeight = ((double)position / (totalValues - 1)) * (MAX_BAR_HEIGHT - MIN_BAR_HEIGHT) + MIN_BAR_HEIGHT;
                    
                    var bar = new Rectangle
                    {
                        Width = barWidth,
                        Height = normalizedHeight,
                        Fill = DefaultColor,
                        Stroke = Brushes.Black,
                        StrokeThickness = 1
                    };

                    var textBlock = new TextBlock
                    {
                        Text = value,
                        TextAlignment = TextAlignment.Center,
                        TextWrapping = TextWrapping.Wrap,
                        Width = barWidth,
                        Foreground = TextColor,
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Center
                    };

                    Canvas.SetLeft(bar, x);
                    Canvas.SetBottom(bar, BAR_MARGIN);
                    Canvas.SetLeft(textBlock, x);
                    Canvas.SetBottom(textBlock, BAR_MARGIN + normalizedHeight + 2);

                    tapeCanvases[0].Children.Add(bar);
                    tapeCanvases[0].Children.Add(textBlock);
                    tapeElements[0].Add((bar, textBlock, GetNumericValue(value)));
                    
                    x += barWidth + BAR_MARGIN;
                }

                StatusText.Text = $"Drew initial array with {valuesList.Count} elements";
                UpdateLayout();
            });
        }

        public void VisualizeDistributionStep(PolyphaseDistributionStep<string> step)
        {
            Dispatcher.Invoke(() =>
            {
                int sourceTape = step.SourceTapeNumber - 1;
                int targetTape = step.TargetTapeNumber - 1;

                // if (sourceTape < 0 || sourceTape >= tapeElements.Count || targetTape < 0 || targetTape >= tapeElements.Count)
                // {
                //     StatusText.Text = $"Error: Invalid tape number. Source: {step.SourceTapeNumber}, Target: {step.TargetTapeNumber}";
                //     return;
                // }

                StatusText.Text = $"Distribution: Moving {step.Value} from tape {step.SourceTapeNumber} to tape {step.TargetTapeNumber}";

                double numericValue = GetNumericValue(step.Value);
                var sourceElement = tapeElements[sourceTape].FirstOrDefault(x => Math.Abs(x.Value - numericValue) < 0.0001);
                if (sourceElement.Bar != null)
                {
                    double x = BAR_MARGIN;
                    if (tapeElements[targetTape].Any())
                    {
                        var lastBar = tapeElements[targetTape].Last();
                        x = Canvas.GetLeft(lastBar.Bar) + lastBar.Bar.Width + BAR_MARGIN;
                    }

                    tapeCanvases[sourceTape].Children.Remove(sourceElement.Bar);
                    tapeCanvases[sourceTape].Children.Remove(sourceElement.Label);
                    tapeElements[sourceTape].Remove(sourceElement);

                    Canvas.SetLeft(sourceElement.Bar, x);
                    Canvas.SetLeft(sourceElement.Label, x);
                    sourceElement.Bar.Fill = DistributionColor;
                    
                    tapeCanvases[targetTape].Children.Add(sourceElement.Bar);
                    tapeCanvases[targetTape].Children.Add(sourceElement.Label);
                    tapeElements[targetTape].Add(sourceElement);

                    UpdateLayout();
                }
            });
        }

        public void VisualizeMergeStep(PolyphaseMergeStep<string> step)
        {
            Dispatcher.Invoke(() =>
            {
                int inputTape = step.InputTape - 1;
                int outputTape = step.OutputTape - 1;

                if (inputTape < 0 || inputTape >= tapeElements.Count || outputTape < 0 || outputTape >= tapeElements.Count)
                {
                    StatusText.Text = $"Error: Invalid tape number. Input: {step.InputTape}, Output: {step.OutputTape}";
                    return;
                }

                StatusText.Text = $"Merge: Moving {step.Value} from tape {step.InputTape} to tape {step.OutputTape}";

                double numericValue = GetNumericValue(step.Value);
                var sourceElement = tapeElements[inputTape].FirstOrDefault(x => Math.Abs(x.Value - numericValue) < 0.0001);
                if (sourceElement.Bar != null)
                {
                    double x = BAR_MARGIN;
                    if (tapeElements[outputTape].Any())
                    {
                        var lastBar = tapeElements[outputTape].Last();
                        x = Canvas.GetLeft(lastBar.Bar) + lastBar.Bar.Width + BAR_MARGIN;
                    }

                    tapeCanvases[inputTape].Children.Remove(sourceElement.Bar);
                    tapeCanvases[inputTape].Children.Remove(sourceElement.Label);
                    tapeElements[inputTape].Remove(sourceElement);

                    Canvas.SetLeft(sourceElement.Bar, x);
                    Canvas.SetLeft(sourceElement.Label, x);
                    sourceElement.Bar.Fill = MergeColor;
                    
                    tapeCanvases[outputTape].Children.Add(sourceElement.Bar);
                    tapeCanvases[outputTape].Children.Add(sourceElement.Label);
                    tapeElements[outputTape].Add(sourceElement);

                    UpdateLayout();
                }
            });
        }

        public void Clear()
        {
            Dispatcher.Invoke(() =>
            {
                foreach (var tape in tapeElements)
                {
                    tape.Clear();
                }
                foreach (var canvas in tapeCanvases)
                {
                    canvas.Children.Clear();
                }
                StatusText.Text = "Cleared visualization";
                UpdateLayout();
            });
        }
    }
}
