using System.Threading.Tasks;
using WpfApp.Models.Steps;

namespace WpfApp.Services
{
    public interface IVisualizationService
    {
        void DrawArray(double[] array);
        Task PreVisualizeStep(SortingStep<double> step) { return Task.CompletedTask; }
        Task VisualizeStep(SortingStep<double> step);
        void LogMessage(string message);
        void ClearLog();
    }
}
