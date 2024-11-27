using System.Threading.Tasks;
using WpfApp.Models.Steps;

namespace WpfApp.Services
{
    public interface IVisualizationService<T>
    {
        void DrawArray(T[] array);
        Task PreVisualizeStep(SortingStep<T> step) { return Task.CompletedTask; }
        Task VisualizeStep(SortingStep<T> step);
        void LogMessage(string message);
        void ClearLog();
    }
}
