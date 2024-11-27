using System.Windows;
using System.Windows.Controls;
using WpfApp.Services;
using WpfApp.ViewModels;

namespace WpfApp.Views
{
    public partial class ExternalSortingWindow : Window
    {
        private readonly ExternalSortingViewModel viewModel;

        public ExternalSortingWindow()
        {
            InitializeComponent();
            var visualizationService = new PolyphaseMergeSortVisualizationService(VisualizationControl, LogListBox);
            viewModel = new ExternalSortingViewModel(visualizationService);
            DataContext = viewModel;
        }

        private void AlgorithmComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox && comboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                viewModel.SelectAlgorithm(selectedItem.Content.ToString());
            }
        }
    }
}
