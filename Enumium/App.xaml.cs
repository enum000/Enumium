using System.Windows;

namespace Enumium
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Services.ReportService.EnsureDataDir();
        }
    }
}
