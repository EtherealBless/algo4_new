using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

namespace WpfApp
{
    public class ColumnInfo
    {
        public string? Name { get; set; }
        public string? SampleData { get; set; }
        public bool IsNumeric { get; set; }
        public int Index { get; set; }
    }

    public partial class CsvSelectorWindow : Window
    {
        private readonly List<string[]> csvData;
        private readonly List<ColumnInfo> columns;
        
        public List<double[]> SelectedColumns { get; private set; } = new();

        public CsvSelectorWindow()
        {
            InitializeComponent();
            csvData = new List<string[]>();
            columns = new List<ColumnInfo>();
        }

        private void OnBrowseClick(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
                FilterIndex = 1
            };

            if (openFileDialog.ShowDialog() == true)
            {
                FilePathTextBox.Text = openFileDialog.FileName;
                LoadCsvFile(openFileDialog.FileName);
            }
        }

        private void LoadCsvFile(string filePath)
        {
            try
            {
                csvData.Clear();
                columns.Clear();

                var lines = File.ReadAllLines(filePath);
                if (lines.Length < 2) // Need at least header and one data row
                {
                    MessageBox.Show("CSV file must contain at least a header row and one data row.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Parse header and data
                var headers = lines[0].Split(',').Select(h => h.Trim()).ToArray();
                for (int i = 1; i < lines.Length; i++)
                {
                    var line = lines[i].Trim();
                    if (!string.IsNullOrEmpty(line))
                    {
                        csvData.Add(line.Split(',').Select(v => v.Trim()).ToArray());
                    }
                }

                // Analyze columns
                for (int i = 0; i < headers.Length; i++)
                {
                    var columnName = headers[i];
                    if (string.IsNullOrEmpty(columnName))
                        columnName = $"Column {i + 1}";

                    var sampleData = csvData.FirstOrDefault()?[i] ?? "";
                    var isNumeric = csvData.All(row => double.TryParse(row[i], out _));

                    columns.Add(new ColumnInfo
                    {
                        Name = columnName,
                        SampleData = sampleData,
                        IsNumeric = isNumeric,
                        Index = i
                    });
                }

                ColumnsListView.ItemsSource = columns;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading CSV file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OnImportClick(object sender, RoutedEventArgs e)
        {
            var selectedColumns = ColumnsListView.SelectedItems.Cast<ColumnInfo>().ToList();
            if (!selectedColumns.Any())
            {
                MessageBox.Show("Please select at least one column to import.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var nonNumericColumns = selectedColumns.Where(c => !c.IsNumeric).ToList();
            if (nonNumericColumns.Any())
            {
                var columnNames = string.Join(", ", nonNumericColumns.Select(c => c.Name));
                MessageBox.Show($"Cannot import non-numeric columns: {columnNames}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                SelectedColumns = selectedColumns.Select(col => 
                    csvData.Select(row => Convert.ToDouble(row[col.Index])).ToArray()
                ).ToList();

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error converting data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OnCancelClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
