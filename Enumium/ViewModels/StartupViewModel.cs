using Enumium.Helpers;
using Enumium.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Enumium.ViewModels
{
    public class StartupViewModel : ViewModelBase
    {
        public ObservableCollection<StartupItemVM> StartupItems { get; } = new();
        public ObservableCollection<Models.ServiceItem> Services { get; } = new();

        private int _selectedTab;
        public int SelectedTab { get => _selectedTab; set { SetProperty(ref _selectedTab, value); } }

        private string _statusText = "";
        public string StatusText { get => _statusText; set => SetProperty(ref _statusText, value); }

        public RelayCommand RefreshCommand { get; }
        public RelayCommand OpenFileLocationCommand { get; }
        public RelayCommand SelectTabCommand { get;  }

        public StartupViewModel()
        {
            RefreshCommand = new RelayCommand(_ => LoadData());
            OpenFileLocationCommand = new RelayCommand(p =>
            {
                if (p is string path && System.IO.File.Exists(path))
                {
                    Process.Start("explorer.exe", $"/select,\"{path}\"");
                }
            });
            SelectTabCommand = new RelayCommand(p => { if (p is string tab) SelectedTab = tab == "Services" ? 1 : 0; });
            LoadData();
        }

        private void LoadData()
        {
            // Startup items
            StartupItems.Clear();
            var items = StartupService.GetStartupItems();
            foreach (var item in items)
                StartupItems.Add(new StartupItemVM(item));

            // Services
            Services.Clear();
            foreach (var svc in TweakService.GetServices())
                Services.Add(svc);

            StatusText = $"Found {items.Count} startup items and {Services.Count} services";
        }
    }

    public class StartupItemVM : ViewModelBase
    {
        public Models.StartupItem Model { get; }

        private bool _isEnabled;
        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                if (SetProperty(ref _isEnabled, value))
                {
                    Model.IsEnabled = value;
                    StartupService.SetStartupEnabled(Model, value);
                    ReportService.LogOptimization(
                        value ? "Enabled" : "Disabled",
                        "Startup", Model.Name, true);
                }
            }
        }

        public string Name => Model.Name;
        public string Command => Model.Command;
        public string Location => Model.Location;
        public string Impact => Model.Impact;
        public string FilePath => Model.FilePath;

        public StartupItemVM(Models.StartupItem model)
        {
            Model = model;
            _isEnabled = model.IsEnabled;
        }
    }
}
