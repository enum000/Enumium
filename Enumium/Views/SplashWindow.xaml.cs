using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Enumium.Services;

namespace Enumium.Views
{
    public partial class SplashWindow : Window
    {
        public SplashWindow()
        {
            InitializeComponent();
        }

        private void English_Click(object sender, MouseButtonEventArgs e)
        {
            SelectLanguage("en");
        }

        private void Russian_Click(object sender, MouseButtonEventArgs e)
        {
            SelectLanguage("ru");
        }

        private async void SelectLanguage(string lang)
        {
            LocalizationService.CurrentLanguage = lang;

            // Hide language buttons, show loading
            EnglishBtn.Visibility = Visibility.Collapsed;
            RussianBtn.Visibility = Visibility.Collapsed;
            ChooseLangText.Visibility = Visibility.Collapsed;
            LoadingPanel.Visibility = Visibility.Visible;

            // Update subtitle with selected language
            SubtitleText.Text = LocalizationService.Get("splash_subtitle");
            LoadingText.Text = LocalizationService.Get("splash_loading");

            // Simulate loading phases
            string[] phases = lang == "en"
                ? new[] { "Loading modules...", "Scanning system...", "Almost ready..." }
                : new[] { "Загрузка модулей...", "Сканирование системы...", "Почти готово..." };

            foreach (var phase in phases)
            {
                LoadingText.Text = phase;
                await Task.Delay(600);
            }

            // Open main window
            var mainWindow = new MainWindow();
            mainWindow.Show();

            // Fade out splash
            var fadeOut = new System.Windows.Media.Animation.DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(400));
            fadeOut.Completed += (s, e) => Close();
            BeginAnimation(OpacityProperty, fadeOut);
        }
    }
}
