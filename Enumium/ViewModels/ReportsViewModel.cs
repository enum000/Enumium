using Enumium.Helpers;
using Enumium.Services;
using System.Collections.ObjectModel;

namespace Enumium.ViewModels
{
    public class ReportsViewModel : ViewModelBase
    {
        public ObservableCollection<Models.OptimizationRecord> History { get; } = new();

        private string _statusText = "";
        public string StatusText { get => _statusText; set => SetProperty(ref _statusText, value); }

        public RelayCommand RefreshCommand { get; }
        public RelayCommand ClearHistoryCommand { get; }
        public RelayCommand ExportCsvCommand { get; }

        public ReportsViewModel()
        {
            RefreshCommand = new RelayCommand(_ => LoadHistory());
            ClearHistoryCommand = new RelayCommand(_ => ClearHistory());
            ExportCsvCommand = new RelayCommand(_ => ExportCsv());
            LoadHistory();
        }

        private void LoadHistory()
        {
            History.Clear();
            foreach (var record in ReportService.LoadHistory().OrderByDescending(r => r.Timestamp))
                History.Add(record);
            StatusText = $"{History.Count} optimization records";
        }

        private void ClearHistory()
        {
            ReportService.ClearHistory();
            History.Clear();
            StatusText = "History cleared";
        }

        private void ExportCsv()
        {
            try
            {
                var csv = ReportService.ExportToCsv(History.ToList());
                var path = System.IO.Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                    $"Enumium_Report_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
                System.IO.File.WriteAllText(path, csv);
                StatusText = $"✅ Report exported to {path}";
            }
            catch (Exception ex)
            {
                StatusText = $"❌ Export failed: {ex.Message}";
            }
        }
    }
}
