using Enumium.Helpers;
using Enumium.Services;
using System.Collections.ObjectModel;

namespace Enumium.ViewModels
{
    public class PrivacyViewModel : ViewModelBase
    {
        public ObservableCollection<PrivacyItemVM> PrivacyItems { get; } = new();

        private double _privacyScore;
        public double PrivacyScore { get => _privacyScore; set => SetProperty(ref _privacyScore, value); }

        private string _statusText = "";
        public string StatusText { get => _statusText; set => SetProperty(ref _statusText, value); }

        public RelayCommand MaxPrivacyCommand { get; }
        public RelayCommand RestoreDefaultsCommand { get; }
        public RelayCommand ApplyCommand { get; }

        public PrivacyViewModel()
        {
            MaxPrivacyCommand = new RelayCommand(_ => SetMaxPrivacy());
            RestoreDefaultsCommand = new RelayCommand(_ => RestoreDefaults());
            ApplyCommand = new RelayCommand(_ => Apply());
            LoadItems();
        }

        private void LoadItems()
        {
            var items = PrivacyService.GetPrivacyItems();
            PrivacyItems.Clear();
            foreach (var item in items)
            {
                item.IsEnabled = PrivacyService.ReadPrivacyState(item);
                PrivacyItems.Add(new PrivacyItemVM(item, UpdateScore));
            }
            UpdateScore();
        }

        private void UpdateScore()
        {
            var models = PrivacyItems.Select(p => p.Model).ToList();
            foreach (var vm in PrivacyItems)
                vm.Model.IsEnabled = vm.IsEnabled;
            PrivacyScore = PrivacyService.CalculatePrivacyScore(models);
        }

        private void SetMaxPrivacy()
        {
            foreach (var item in PrivacyItems)
                item.IsEnabled = true;
            UpdateScore();
            StatusText = "All privacy protections enabled. Click Apply to save.";
        }

        private void RestoreDefaults()
        {
            foreach (var item in PrivacyItems)
                item.IsEnabled = false;
            UpdateScore();
            StatusText = "Privacy settings restored to Windows defaults. Click Apply to save.";
        }

        private void Apply()
        {
            int applied = 0;
            foreach (var item in PrivacyItems)
            {
                if (PrivacyService.ApplyPrivacy(item.Model, item.IsEnabled))
                    applied++;
            }
            StatusText = $"✅ {applied} privacy settings applied successfully!";
            ReportService.LogOptimization("Privacy Update", "Privacy",
                $"Applied {applied} privacy settings (Score: {PrivacyScore}%)", true);
        }
    }

    public class PrivacyItemVM : ViewModelBase
    {
        public Models.PrivacyItem Model { get; }
        private readonly Action? _onChanged;

        private bool _isEnabled;
        public bool IsEnabled
        {
            get => _isEnabled;
            set { SetProperty(ref _isEnabled, value); Model.IsEnabled = value; _onChanged?.Invoke(); }
        }

        public string Name => Model.Name;
        public string Description => Model.Description;
        public string Category => Model.Category;

        public PrivacyItemVM(Models.PrivacyItem model, Action? onChanged = null)
        {
            Model = model;
            _isEnabled = model.IsEnabled;
            _onChanged = onChanged;
        }
    }
}
