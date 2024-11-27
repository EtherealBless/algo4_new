using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using WpfApp.Models.Steps;

namespace WpfApp.Services
{
    public class MergeSortVisualizationService : IVisualizationService<double>
    {
        private readonly Visualization.MergeSortVisualization visualization;
        private readonly ListBox logList;

        public MergeSortVisualizationService(Visualization.MergeSortVisualization visualization, ListBox logList)
        {
            this.visualization = visualization;
            this.logList = logList;
        }

        public void DrawArray(double[] array)
        {
            visualization.InitializeArrays(array);
        }

        public Task PreVisualizeStep(SortingStep<double> step)
        {
            return Task.CompletedTask;
        }

        public async Task VisualizeStep(SortingStep<double> step)
        {
            visualization.UpdateVisualization(step);
            LogMessage(step.GetDescription());
            await Task.Delay(step.DelayMilliseconds);
        }

        public void LogMessage(string message)
        {
            logList.Items.Add($"[{DateTime.Now:HH:mm:ss}] {message}");
            logList.ScrollIntoView(logList.Items[^1]);
        }

        public void ClearLog()
        {
            logList.Items.Clear();
        }
    }
}
