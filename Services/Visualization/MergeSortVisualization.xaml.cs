// using System;
// using System.Linq;
// using System.Windows;
// using System.Windows.Controls;
// using System.Windows.Media;
// using System.Windows.Shapes;
// using WpfApp.Models.Steps;

// namespace WpfApp.Services.Visualization
// {
//     public partial class MergeSortVisualization : UserControl
//     {
//         private readonly SolidColorBrush CompareColor = new(Colors.Red);
//         private readonly SolidColorBrush MergeColor = new(Colors.Purple);
//         private readonly SolidColorBrush DefaultColor = new(Colors.Blue);
//         private readonly SolidColorBrush TempColor = new(Colors.Green);
//         private Rectangle[] mainArrayRects;
//         private Rectangle[] tempArrayRects;

//         public MergeSortVisualization()
//         {
//             InitializeComponent();
//         }

//         public void InitializeArrays(double[] array)
//         {
//             MainArrayCanvas.Children.Clear();
//             TempArrayCanvas.Children.Clear();

//             double maxValue = array.Max();
//             double minValue = array.Min();
//             double range = maxValue - minValue;
//             double rectWidth = MainArrayCanvas.ActualWidth / array.Length;
            
//             mainArrayRects = new Rectangle[array.Length];
//             tempArrayRects = new Rectangle[array.Length];

//             for (int i = 0; i < array.Length; i++)
//             {
//                 // Create rectangle for main array
//                 var mainRect = CreateRectangle(array[i], maxValue, minValue, rectWidth, i, MainArrayCanvas.ActualHeight);
//                 MainArrayCanvas.Children.Add(mainRect);
//                 mainArrayRects[i] = mainRect;

//                 // Create rectangle for temp array (initially hidden)
//                 var tempRect = CreateRectangle(array[i], maxValue, minValue, rectWidth, i, TempArrayCanvas.ActualHeight);
//                 tempRect.Opacity = 0;
//                 TempArrayCanvas.Children.Add(tempRect);
//                 tempArrayRects[i] = tempRect;
//             }
//         }

//         private Rectangle CreateRectangle(double value, double maxValue, double minValue, double width, int index, double canvasHeight)
//         {
//             double height = ((value - minValue) / (maxValue - minValue)) * canvasHeight;
//             var rect = new Rectangle
//             {
//                 Width = width * 0.9,
//                 Height = height,
//                 Fill = DefaultColor
//             };

//             Canvas.SetLeft(rect, index * width);
//             Canvas.SetBottom(rect, 0);
//             return rect;
//         }

//         public void UpdateVisualization(SortingStep<double> step)
//         {
//             // Reset colors
//             foreach (var rect in mainArrayRects)
//                 rect.Fill = DefaultColor;
//             foreach (var rect in tempArrayRects)
//                 rect.Fill = TempColor;

//             if (step is MergeStep<double> mergeStep)
//             {
//                 // Show the merging process
//                 for (int i = mergeStep.LeftStart; i < mergeStep.LeftStart + mergeStep.Length; i++)
//                 {
//                     if (i < mergeStep.RightStart)
//                         mainArrayRects[i].Fill = CompareColor;
//                     else
//                         mainArrayRects[i].Fill = MergeColor;

//                     // Show corresponding elements in temp array
//                     tempArrayRects[i].Opacity = 1;
//                 }
//             }

//             // Update the array values
//             double maxValue = step.CurrentArray.Max();
//             double minValue = step.CurrentArray.Min();
//             double range = maxValue - minValue;

//             for (int i = 0; i < step.CurrentArray.Length; i++)
//             {
//                 double height = ((step.CurrentArray[i] - minValue) / range) * MainArrayCanvas.ActualHeight;
//                 mainArrayRects[i].Height = height;
//                 tempArrayRects[i].Height = height;
//             }
//         }
//     }
// }
