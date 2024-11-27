using System.Threading.Tasks;
using System.Windows;
using WpfApp.Models.Steps;

namespace WpfApp.Services.Visualization
{
    public partial class PolyphaseMergeSortVisualizationWindow : Window
    {
        public PolyphaseMergeSortVisualization Visualization => _visualization;

        public PolyphaseMergeSortVisualizationWindow()
        {
            InitializeComponent();
        }
    }
}
