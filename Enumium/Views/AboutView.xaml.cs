using System.Windows.Controls;
using System.Windows.Input;
using Enumium.ViewModels;

namespace Enumium.Views
{
    public partial class AboutView : UserControl
    {
        public AboutView() { InitializeComponent(); }

        private void GitHub_Click(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is AboutViewModel vm)
                vm.OpenGitHubCommand.Execute(null);
        }
    }
}
