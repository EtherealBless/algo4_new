using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using WpfApp.Models.Steps;
using WpfApp.Services.Visualization;

namespace WpfApp.Services
{
    public class PolyphaseMergeSortVisualizationService : IVisualizationService<string>, IDisposable
    {
        private readonly PolyphaseMergeSortVisualization visualization;
        private readonly ListBox logList;
        private readonly List<FileStream> openFiles;
        private bool isInitialized;
        private bool isDisposed;

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

        public PolyphaseMergeSortVisualizationService(PolyphaseMergeSortVisualization visualization, ListBox logList)
        {
            this.visualization = visualization ?? throw new ArgumentNullException(nameof(visualization));
            this.logList = logList ?? throw new ArgumentNullException(nameof(logList));
            this.openFiles = new List<FileStream>();
            this.isInitialized = false;
            this.isDisposed = false;
        }

        private void CloseAllFiles()
        {
            // First, close all open file handles
            foreach (var file in openFiles.ToList())
            {
                try
                {
                    if (file != null)
                    {
                        string filePath = file.Name;
                        file.Close();
                        file.Dispose();
                        openFiles.Remove(file);

                        // Try to delete the file after closing
                        if (File.Exists(filePath))
                        {
                            File.Delete(filePath);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogMessage($"Warning: Error closing file: {ex.Message}");
                }
            }
            openFiles.Clear();

            // Then try to clean up any remaining temporary files
            for (int i = 0; i < 10; i++) // Assuming max 10 tapes
            {
                try
                {
                    string fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"tape_{i}.TMP");
                    if (File.Exists(fileName))
                    {
                        File.Delete(fileName);
                    }
                }
                catch (Exception ex)
                {
                    LogMessage($"Warning: Error deleting temporary file: {ex.Message}");
                }
            }
        }

        public void Initialize(int numberOfTapes)
        {
            if (isDisposed)
            {
                throw new ObjectDisposedException(nameof(PolyphaseMergeSortVisualizationService));
            }

            if (!isInitialized)
            {
                CloseAllFiles(); // Ensure any previous files are closed
                if (visualization != null)
                {
                    visualization.Initialize(numberOfTapes);
                }
                isInitialized = true;
            }
        }

        public void Cleanup()
        {
            if (!isDisposed)
            {
                CloseAllFiles();
                isInitialized = false;
            }
        }

        public void DrawArray(string[] array)
        {
            if (isDisposed)
            {
                throw new ObjectDisposedException(nameof(PolyphaseMergeSortVisualizationService));
            }

            // Convert strings to numeric values for visualization
            visualization.DrawInitialArray(array);
        }

        public void DrawInitialArray(IEnumerable<string> values)
        {
            if (visualization != null)
            {
                visualization.DrawInitialArray(values);
            }
        }

        public async Task VisualizeStep(SortingStep<string> step)
        {
            if (isDisposed)
            {
                throw new ObjectDisposedException(nameof(PolyphaseMergeSortVisualizationService));
            }

            try
            {
                if (step is PolyphaseDistributionStep<string> distributionStep)
                {
                    await HandleDistributionStep(distributionStep);
                }
                else if (step is PolyphaseMergeStep<string> mergeStep)
                {
                    await HandleMergeStep(mergeStep);
                }
            }
            catch (Exception ex)
            {
                LogMessage($"Error during visualization: {ex.Message}");
            }
        }

        public async Task HandleDistributionStep(PolyphaseDistributionStep<string> step)
        {
            if (isDisposed)
            {
                throw new ObjectDisposedException(nameof(PolyphaseMergeSortVisualizationService));
            }

            try
            {
                await visualization.Dispatcher.InvokeAsync(() =>
                {
                    visualization.VisualizeDistributionStep(step);
                    LogMessage($"Distribution: Moving {step.Value} from tape {step.SourceTapeNumber} to tape {step.TargetTapeNumber}");
                });
            }
            catch (Exception ex)
            {
                LogMessage($"Error during distribution: {ex.Message}");
            }
        }

        public async Task HandleMergeStep(PolyphaseMergeStep<string> step)
        {
            if (isDisposed)
            {
                throw new ObjectDisposedException(nameof(PolyphaseMergeSortVisualizationService));
            }

            try
            {
                double value = GetNumericValue(step.Value);
                var numericStep = new PolyphaseMergeStep<double>(
                    step.InputTape,
                    step.OutputTape,
                    step.InputIndex,
                    step.OutputIndex,
                    value.ToString()
                );
                await visualization.Dispatcher.InvokeAsync(() =>
                {
                    visualization.VisualizeMergeStep(step);
                    LogMessage($"Merge: Moving {step.Value} from tape {step.InputTape} to tape {step.OutputTape}");
                });
            }
            catch (Exception ex)
            {
                LogMessage($"Error during merge: {ex.Message}");
            }
        }

        public void LogMessage(string message)
        {
            if (logList != null)
            {
                logList.Dispatcher.Invoke(() =>
                {
                    logList.Items.Add(message);
                    logList.ScrollIntoView(logList.Items[logList.Items.Count - 1]);
                });
            }
        }

        public void ClearLog()
        {
            try
            {
                logList.Dispatcher.Invoke(() => logList.Items.Clear());
            }
            catch (Exception)
            {
                // Ignore logging errors during cleanup
            }
        }

        public void Dispose()
        {
            if (!isDisposed)
            {
                Cleanup();
                isDisposed = true;
                GC.SuppressFinalize(this);
            }
        }

        ~PolyphaseMergeSortVisualizationService()
        {
            Dispose();
        }
    }
}
