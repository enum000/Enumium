using Enumium.Helpers;
using Enumium.Services;
using System.Collections.ObjectModel;

namespace Enumium.ViewModels
{
    public class PerformanceViewModel : ViewModelBase
    {
        public ObservableCollection<TweakItemVM> AllTweaks { get; } = new();
        public ObservableCollection<TweakItemVM> FilteredTweaks { get; } = new();

        private string _selectedCategory = "System";
        public string SelectedCategory
        {
            get => _selectedCategory;
            set { SetProperty(ref _selectedCategory, value); FilterTweaks(); }
        }

        private string _statusMessage = "";
        public string StatusMessage { get => _statusMessage; set => SetProperty(ref _statusMessage, value); }

        public RelayCommand SelectCategoryCommand { get; }
        public RelayCommand ApplyAllCommand { get; }
        public RelayCommand ResetAllCommand { get; }

        public PerformanceViewModel()
        {
            SelectCategoryCommand = new RelayCommand(p => { if (p is string cat) SelectedCategory = cat; });
            ApplyAllCommand = new RelayCommand(_ => ApplyAll());
            ResetAllCommand = new RelayCommand(_ => ResetAll());

            LoadTweaks();
        }

        private void LoadTweaks()
        {
            var tweaks = TweakService.GetSystemTweaks();
            AllTweaks.Clear();
            foreach (var t in tweaks)
            {
                t.IsEnabled = TweakService.ReadTweakState(t);
                t.OriginalValue = t.IsEnabled;
                AllTweaks.Add(new TweakItemVM(t));
            }
            FilterTweaks();
        }

        private void FilterTweaks()
        {
            FilteredTweaks.Clear();
            foreach (var t in AllTweaks.Where(t => t.Model.Category == SelectedCategory))
                FilteredTweaks.Add(t);
        }

        private void ApplyAll()
        {
            int applied = 0;
            foreach (var t in AllTweaks)
            {
                if (t.IsEnabled != t.Model.OriginalValue)
                {
                    if (TweakService.ApplyTweak(t.Model, t.IsEnabled))
                    {
                        t.Model.OriginalValue = t.IsEnabled;
                        applied++;
                        ReportService.LogOptimization(
                            t.IsEnabled ? "Enabled" : "Disabled",
                            "Performance", t.Model.Name, true);
                    }
                }
            }
            StatusMessage = applied > 0 ? $"✅ {applied} tweak(s) applied successfully!" : "No changes to apply.";
        }

        private void ResetAll()
        {
            foreach (var t in FilteredTweaks)
            {
                t.IsEnabled = false;
                TweakService.ApplyTweak(t.Model, false);
            }
            StatusMessage = "All tweaks in this category have been reset.";
        }
    }

    public class TweakItemVM : ViewModelBase
    {
        public Models.TweakItem Model { get; }

        private bool _isEnabled;
        public bool IsEnabled
        {
            get => _isEnabled;
            set { SetProperty(ref _isEnabled, value); Model.IsEnabled = value; }
        }

        public string Name => Model.Name;
        public string Description => Model.Description;
        public string Impact => Model.Impact;
        public string Category => Model.Category;

        public TweakItemVM(Models.TweakItem model)
        {
            Model = model;
            _isEnabled = model.IsEnabled;
        }
    }
}
