using Enumium.Helpers;
using Enumium.Services;
using System.Collections.ObjectModel;

namespace Enumium.ViewModels
{
    public class CleanerViewModel : ViewModelBase
    {
        public ObservableCollection<Models.CleanerItem> ScanResults { get; } = new();

        private bool _isScanning;
        public bool IsScanning { get => _isScanning; set => SetProperty(ref _isScanning, value); }

        private bool _isCleaning;
        public bool IsCleaning { get => _isCleaning; set => SetProperty(ref _isCleaning, value); }

        private bool _hasScanResults;
        public bool HasScanResults { get => _hasScanResults; set => SetProperty(ref _hasScanResults, value); }

        private string _statusText = "Click 'Scan Now' to find junk files";
        public string StatusText { get => _statusText; set => SetProperty(ref _statusText, value); }

        private string _scanProgress = "";
        public string ScanProgress { get => _scanProgress; set => SetProperty(ref _scanProgress, value); }

        private long _totalSize;
        public long TotalSize { get => _totalSize; set => SetProperty(ref _totalSize, value); }

        private int _totalFiles;
        public int TotalFiles { get => _totalFiles; set => SetProperty(ref _totalFiles, value); }

        private long _freedSpace;
        public long FreedSpace { get => _freedSpace; set => SetProperty(ref _freedSpace, value); }

        public RelayCommand ScanCommand { get; }
        public RelayCommand CleanCommand { get; }
        public RelayCommand SelectAllCommand { get; }
        public RelayCommand DeselectAllCommand { get; }

        public CleanerViewModel()
        {
            ScanCommand = new RelayCommand(async _ => await ScanAsync());
            CleanCommand = new RelayCommand(async _ => await CleanAsync(), _ => HasScanResults && !IsCleaning);
            SelectAllCommand = new RelayCommand(_ => SetAll(true));
            DeselectAllCommand = new RelayCommand(_ => SetAll(false));
        }

        private async Task ScanAsync()
        {
            IsScanning = true;
            HasScanResults = false;
            ScanResults.Clear();
            StatusText = "Scanning...";

            var progress = new Progress<string>(msg => ScanProgress = msg);
            var results = await CleanerService.ScanAsync(progress);

            foreach (var item in results)
                ScanResults.Add(item);

            TotalSize = results.Sum(r => r.Size);
            TotalFiles = results.Sum(r => r.FileCount);
            HasScanResults = results.Count > 0;
            StatusText = HasScanResults
                ? $"Found {TotalFiles:N0} files ({FormatSize(TotalSize)}) in {results.Count} categories"
                : "Your system is clean! No junk files found.";
            IsScanning = false;
        }

        private async Task CleanAsync()
        {
            IsCleaning = true;
            StatusText = "Cleaning...";
            var progress = new Progress<string>(msg => ScanProgress = msg);
            var selectedItems = ScanResults.Where(i => i.IsSelected).ToList();
            FreedSpace = await CleanerService.CleanAsync(selectedItems, progress);
            StatusText = $"✅ Cleanup complete! Freed {FormatSize(FreedSpace)}";
            ReportService.LogOptimization("Junk Cleanup", "Cleaner",
                $"Freed {FormatSize(FreedSpace)} from {selectedItems.Count} categories", true);

            // Re-scan
            HasScanResults = false;
            IsCleaning = false;
        }

        private void SetAll(bool selected)
        {
            foreach (var item in ScanResults)
                item.IsSelected = selected;
        }

        private static string FormatSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1) { order++; len /= 1024; }
            return $"{len:0.##} {sizes[order]}";
        }
    }
}
