using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using WpfApp.Algorithms;
using WpfApp.Models.Steps;
using WpfApp.Services;
using WpfApp.Services.Visualization;

namespace WpfApp.ViewModels
{
    public class SortingViewModel : INotifyPropertyChanged
    {
        private IVisualizationService<double> visualizationService = null!;
        private ISortingAlgorithm<double>? currentAlgorithm = null!;
        private IExternalSortingAlgorithm<string>? currentExternalAlgorithm = null!;
        private List<double> data = null!;
        private List<SortingStep<double>> sortingSteps = null!;
        private int currentStepIndex = -1;
        private bool isSorting;

        public event PropertyChangedEventHandler? PropertyChanged;

        public SortingViewModel() { }
        public bool IsSorting
        {
            get => isSorting;
            private set
            {
                isSorting = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanStart));
                OnPropertyChanged(nameof(CanStop));
            }
        }

        public bool CanStart => !IsSorting && data?.Count > 0 && currentAlgorithm != null;
        public bool CanStop => IsSorting;
        public bool CanStepForward => sortingSteps != null && currentStepIndex < sortingSteps.Count - 1;
        public bool CanStepBack => sortingSteps != null && currentStepIndex > 0;

        public void SetData(IEnumerable<double> newData)
        {
            data = newData.ToList();
            ResetVisualization();
            visualizationService?.DrawArray(data.ToArray());
            OnPropertyChanged(nameof(CanStart));
        }

        public void SelectAlgorithm(string algorithmName, IVisualizationService<double> newVisualizationService)
        {
            visualizationService?.ClearLog();
            visualizationService = newVisualizationService;
            currentAlgorithm = algorithmName switch
            {
                "Selection Sort" => new SelectionSort<double>(),
                "Insertion Sort" => new InsertionSort<double>(),
                "Shell Sort" => new ShellSort<double>(),
                "Quick Sort" => new QuickSort<double>(),
                "Merge Sort" => new MergeSort<double>(),
                "Natural Merge Sort" => new NaturalMergeSort<double>(),
                _ => null
            };
            ResetVisualization();
            visualizationService.LogMessage($"Selected algorithm: {algorithmName}");
            OnPropertyChanged(nameof(CanStart));
        }

        public void StartSorting()
        {
            IsSorting = true;
        }

        public void StopSorting()
        {
            IsSorting = false;
        }

        public async Task StepForward()
        {
            if (CanStepForward)
            {
                Debug.WriteLine($"Stepping forward to step {currentStepIndex + 1}");
                currentStepIndex++;
                try
                {
                    await visualizationService.VisualizeStep(sortingSteps[currentStepIndex]);
                    Debug.WriteLine("Step visualization completed");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error in StepForward: {ex}");
                    throw;
                }
                OnPropertyChanged(nameof(CanStepForward));
                OnPropertyChanged(nameof(CanStepBack));
            }
        }

        public async Task StepBack()
        {
            if (CanStepBack)
            {
                currentStepIndex--;
                await visualizationService.VisualizeStep(sortingSteps[currentStepIndex]);
                OnPropertyChanged(nameof(CanStepForward));
                OnPropertyChanged(nameof(CanStepBack));
            }
        }

        private void ResetVisualization()
        {
            if (data != null && currentAlgorithm != null)
            {
                Debug.WriteLine("ResetVisualization started");
                var dataArray = data.ToArray();
                Debug.WriteLine("Creating sorting steps");
                sortingSteps = new List<SortingStep<double>>(currentAlgorithm.Sort(dataArray));
                Debug.WriteLine($"Created {sortingSteps.Count} sorting steps");
                currentStepIndex = -1;

                // Initialize visualization based on algorithm type
                if (visualizationService is PolyphaseMergeSortVisualizationService polyphaseService)
                {
                    Debug.WriteLine("Initializing polyphase visualization");
                    polyphaseService.Initialize(3);
                }

                Debug.WriteLine("Drawing initial array");
                visualizationService?.DrawArray(dataArray);
                OnPropertyChanged(nameof(CanStepForward));
                OnPropertyChanged(nameof(CanStepBack));
                Debug.WriteLine("ResetVisualization completed");
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
