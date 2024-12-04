using System.Threading.Tasks;
using WpfApp.Models.Steps;

namespace WpfApp.Services
{
    public interface IArrayVisualizationService<T> : IVisualizationService<T>
    {
        void DrawArray(T[] array);
    }

    public interface IVisualizationService<T>
    {
        public Task PreVisualizeStep(SortingStep<T> step) { return Task.CompletedTask; }
        public Task VisualizeStep(SortingStep<T> step);
        public void LogMessage(string message);
        public void ClearLog();
    }
}
