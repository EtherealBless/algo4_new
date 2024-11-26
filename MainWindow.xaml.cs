using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Microsoft.Win32;
using WpfApp.Services;
using WpfApp.Services.Visualization;
using WpfApp.ViewModels;

namespace WpfApp
{
    public partial class MainWindow : Window
    {
        private readonly SortingViewModel viewModel;
        private DispatcherTimer timer = null!;

        public MainWindow()
        {
            InitializeComponent();
            viewModel = new SortingViewModel();
            DataContext = viewModel;
            InitializeTimer();
            AllocConsole();
        }

        [System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError = true)]
        [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
        static extern bool AllocConsole();

        private void InitializeTimer()
        {
            timer = new DispatcherTimer();
            timer.Tick += Timer_Tick;
            timer.Interval = TimeSpan.FromMilliseconds(SpeedSlider.Value);
            SpeedSlider.ValueChanged += (s, e) => timer.Interval = TimeSpan.FromMilliseconds(e.NewValue);
        }

        private async void Timer_Tick(object? sender, EventArgs e)
        {
            if (viewModel.CanStepForward)
            {
                await viewModel.StepForward();
            }
            else
            {
                timer.Stop();
                viewModel.StopSorting();
            }
        }

        private void OnLoadCsvClick(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
                FilterIndex = 1
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    var lines = File.ReadAllLines(openFileDialog.FileName);
                    var data = lines
                        .Select(line => double.TryParse(line.Trim(), out double value) ? value : double.NaN)
                        .Where(value => !double.IsNaN(value));
                    
                    viewModel.SetData(data);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void OnExitClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OnAlgorithmSelect(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem)
            {
                var algorithmName = menuItem.Header.ToString();
                IVisualizationService visualizationService = algorithmName switch
                {
                    "Selection Sort" => new SelectionSortVisualization(VisualizationCanvas, LogList),
                    "Insertion Sort" => new InsertionSortVisualization(VisualizationCanvas, LogList),
                    "Shell Sort" => new ShellSortVisualization(VisualizationCanvas, LogList),
                    "Quick Sort" => new QuickSortVisualization(VisualizationCanvas, LogList),
                    _ => throw new NotImplementedException()
                };

                viewModel.SelectAlgorithm(algorithmName, visualizationService);
            }
        }

        private void OnStartClick(object sender, RoutedEventArgs e)
        {
            if (viewModel.CanStart)
            {
                viewModel.StartSorting();
                timer.Start();
            }
            else if (!viewModel.IsSorting)
            {
                MessageBox.Show(
                    "Please load data and select an algorithm first.", 
                    "Cannot Start", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Warning);
            }
        }

        private void OnStopClick(object sender, RoutedEventArgs e)
        {
            if (viewModel.CanStop)
            {
                timer.Stop();
                viewModel.StopSorting();
            }
        }

        private async void OnStepForwardClick(object sender, RoutedEventArgs e)
        {
            if (viewModel.CanStepForward)
            {
                await viewModel.StepForward();
            }
        }

        private async void OnStepBackClick(object sender, RoutedEventArgs e)
        {
            if (viewModel.CanStepBack)
            {
                await viewModel.StepBack();
            }
        }
    }
}
