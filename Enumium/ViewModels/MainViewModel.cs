using Enumium.Helpers;
using Enumium.Services;

namespace Enumium.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private ViewModelBase? _currentPage;
        public ViewModelBase? CurrentPage
        {
            get => _currentPage;
            set => SetProperty(ref _currentPage, value);
        }

        private string _currentPageName = "Dashboard";
        public string CurrentPageName
        {
            get => _currentPageName;
            set => SetProperty(ref _currentPageName, value);
        }

        // Localized navigation labels
        public string NavDashboard => LocalizationService.Get("nav_dashboard");
        public string NavPerformance => LocalizationService.Get("nav_performance");
        public string NavCleaner => LocalizationService.Get("nav_cleaner");
        public string NavStartup => LocalizationService.Get("nav_startup");
        public string NavNetwork => LocalizationService.Get("nav_network");
        public string NavPrivacy => LocalizationService.Get("nav_privacy");
        public string NavTools => LocalizationService.Get("nav_tools");
        public string NavReports => LocalizationService.Get("nav_reports");
        public string NavAbout => LocalizationService.Get("nav_about");

        // Lazy-loaded ViewModels
        private DashboardViewModel? _dashboard;
        private PerformanceViewModel? _performance;
        private CleanerViewModel? _cleaner;
        private StartupViewModel? _startup;
        private NetworkViewModel? _network;
        private PrivacyViewModel? _privacy;
        private ToolsViewModel? _tools;
        private ReportsViewModel? _reports;
        private AboutViewModel? _about;

        public RelayCommand NavigateCommand { get; }
        public RelayCommand MinimizeCommand { get; }
        public RelayCommand MaximizeCommand { get; }
        public RelayCommand CloseCommand { get; }

        public MainViewModel()
        {
            NavigateCommand = new RelayCommand(p =>
            {
                if (p is string page) NavigateTo(page);
            });

            MinimizeCommand = new RelayCommand(_ =>
            {
                if (System.Windows.Application.Current.MainWindow != null)
                    System.Windows.Application.Current.MainWindow.WindowState = System.Windows.WindowState.Minimized;
            });

            MaximizeCommand = new RelayCommand(_ =>
            {
                if (System.Windows.Application.Current.MainWindow != null)
                {
                    var win = System.Windows.Application.Current.MainWindow;
                    win.WindowState = win.WindowState == System.Windows.WindowState.Maximized
                        ? System.Windows.WindowState.Normal
                        : System.Windows.WindowState.Maximized;
                }
            });

            CloseCommand = new RelayCommand(_ =>
            {
                System.Windows.Application.Current.Shutdown();
            });

            NavigateTo("Dashboard");
        }

        public void NavigateTo(string page)
        {
            CurrentPageName = page;
            CurrentPage = page switch
            {
                "Dashboard" => _dashboard ??= new DashboardViewModel(),
                "Performance" => _performance ??= new PerformanceViewModel(),
                "Cleaner" => _cleaner ??= new CleanerViewModel(),
                "Startup" => _startup ??= new StartupViewModel(),
                "Network" => _network ??= new NetworkViewModel(),
                "Privacy" => _privacy ??= new PrivacyViewModel(),
                "Tools" => _tools ??= new ToolsViewModel(),
                "Reports" => _reports ??= new ReportsViewModel(),
                "About" => _about ??= new AboutViewModel(),
                _ => _dashboard ??= new DashboardViewModel()
            };
        }
    }
}
