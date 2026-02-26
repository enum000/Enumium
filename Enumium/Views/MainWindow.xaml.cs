using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using Enumium.Helpers;

namespace Enumium.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            EnableAcrylicEffect();
        }

        private void EnableAcrylicEffect()
        {
            try
            {
                var hwnd = new WindowInteropHelper(this).Handle;

                // Enable dark mode for window
                int useDarkMode = 1;
                NativeMethods.DwmSetWindowAttribute(hwnd, NativeMethods.DWMWA_USE_IMMERSIVE_DARK_MODE,
                    ref useDarkMode, sizeof(int));

                // Try Mica/Acrylic backdrop (Windows 11)
                int backdropType = 2; // DWMSBT_MAINWINDOW (Mica)
                NativeMethods.DwmSetWindowAttribute(hwnd, NativeMethods.DWMWA_SYSTEMBACKDROP_TYPE,
                    ref backdropType, sizeof(int));

                // Extend frame
                var margins = new NativeMethods.MARGINS(-1, -1, -1, -1);
                NativeMethods.DwmExtendFrameIntoClientArea(hwnd, ref margins);
            }
            catch { /* Graceful fallback for older Windows versions */ }
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                WindowState = WindowState == WindowState.Maximized
                    ? WindowState.Normal : WindowState.Maximized;
            }
            else
            {
                DragMove();
            }
        }
    }
}
