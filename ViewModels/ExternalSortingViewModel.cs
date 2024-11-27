using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Win32;
using WpfApp.Algorithms;
using WpfApp.Models.Steps;
using WpfApp.Services;
using WpfApp.Commands;
using System.Linq;
using System.Windows;

namespace WpfApp.ViewModels
{
    public class ExternalSortingViewModel : INotifyPropertyChanged
    {
        private readonly IVisualizationService<string> visualizationService;
        private IExternalSortingAlgorithm<string> currentAlgorithm;
        private string inputFilePath;
        private string outputFilePath;
        private bool isRunning;
        private int delayMs = 100;
        private string windowTitle = "External Sorting";

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand SelectOutputFileCommand { get; }
        public ICommand StartCommand { get; }
        public ICommand StepForwardCommand { get; }
        public ICommand StepBackCommand { get; }
        public ICommand ResetCommand { get; }
        public ICommand LoadCsvCommand { get; }
        public ICommand ExitCommand { get; }

        public string WindowTitle
        {
            get => windowTitle;
            set
            {
                windowTitle = value;
                OnPropertyChanged();
            }
        }

        public string InputFilePath
        {
            get => inputFilePath;
            set
            {
                inputFilePath = value;
                WindowTitle = string.IsNullOrEmpty(value) ? 
                    "External Sorting" : 
                    $"External Sorting - {Path.GetFileName(value)}";
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanStart));
            }
        }

        public string OutputFilePath
        {
            get => outputFilePath;
            set
            {
                outputFilePath = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanStart));
            }
        }

        public bool IsRunning
        {
            get => isRunning;
            set
            {
                isRunning = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanStart));
            }
        }

        public bool CanStart => !string.IsNullOrEmpty(InputFilePath) &&
                              !string.IsNullOrEmpty(OutputFilePath) &&
                              !IsRunning &&
                              currentAlgorithm != null;

        public ExternalSortingViewModel(IVisualizationService<string> visualizationService)
        {
            this.visualizationService = visualizationService ?? throw new ArgumentNullException(nameof(visualizationService));

            SelectOutputFileCommand = new RelayCommand(_ => SelectOutputFile());
            StartCommand = new RelayCommand(_ => StartSorting(), _ => CanStart);
            StepForwardCommand = new RelayCommand(_ => StepForward());
            StepBackCommand = new RelayCommand(_ => StepBack());
            ResetCommand = new RelayCommand(_ => Reset());
            LoadCsvCommand = new RelayCommand(_ => LoadCsv());
            ExitCommand = new RelayCommand(_ => Exit());

            // Initialize with PolyphaseMergeSort by default
            SelectAlgorithm("Polyphase Merge Sort");
        }

        public void SelectAlgorithm(string algorithmName)
        {
            switch (algorithmName)
            {
                case "Polyphase Merge Sort":
                    currentAlgorithm = new PolyphaseMergeSort(4); // Using 4 tapes
                    if (visualizationService is PolyphaseMergeSortVisualizationService visualService)
                    {
                        visualService.Initialize(4); // Initialize visualization with 4 tapes
                    }
                    break;
                // Add other algorithms here
                default:
                    currentAlgorithm = null;
                    break;
            }
            Reset();
            OnPropertyChanged(nameof(CanStart));
        }

        private void SelectOutputFile()
        {
            var dialog = new SaveFileDialog
            {
                Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
                Title = "Select Output File"
            };

            if (dialog.ShowDialog() == true)
            {
                OutputFilePath = dialog.FileName;
            }
        }

        private async Task StartSorting()
        {
            if (currentAlgorithm == null || string.IsNullOrEmpty(InputFilePath) || string.IsNullOrEmpty(OutputFilePath))
                return;

            try
            {
                IsRunning = true;

                // Start sorting - let the algorithm handle file reading
                foreach (var step in currentAlgorithm.Sort(InputFilePath, OutputFilePath, StringComparer.Ordinal))
                {
                    await Task.Delay(delayMs);
                    await Task.Run(() => visualizationService.VisualizeStep(step));
                }

                IsRunning = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during sorting: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                IsRunning = false;
            }
        }

        private void StepForward()
        {
            // Implement step forward logic
        }

        private void StepBack()
        {
            // Implement step back logic
        }

        private void Reset()
        {
            IsRunning = false;
        }

        private void LoadCsv()
        {
            try
            {
                var dialog = new OpenFileDialog
                {
                    Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
                    Title = "Select CSV File"
                };

                if (dialog.ShowDialog() == true)
                {
                    // Create a temporary file and stream the data
                    string tempFile = Path.GetTempFileName();
                    var values = new List<string>();

                    using (var reader = new StreamReader(dialog.FileName))
                    using (var writer = new StreamWriter(tempFile))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            line = line.Trim();
                            if (!string.IsNullOrEmpty(line))
                            {
                                writer.WriteLine(line);
                                values.Add(line);
                            }
                        }
                    }

                    InputFilePath = tempFile;

                    // Draw initial array in visualization
                    if (visualizationService is PolyphaseMergeSortVisualizationService visualService)
                    {
                        visualService.DrawInitialArray(values);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading CSV file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Exit()
        {
            Application.Current.MainWindow?.Close();
        }

        private void SetData(double[] data)
        {
            // Set default output file path if not set
            if (string.IsNullOrEmpty(OutputFilePath))
            {
                OutputFilePath = Path.Combine(
                    Path.GetDirectoryName(InputFilePath) ?? "",
                    "sorted_" + Path.GetFileName(InputFilePath));
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
