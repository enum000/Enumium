using System.Windows.Controls;
using System.Windows.Input;
using Enumium.ViewModels;

namespace Enumium.Views
{
    public partial class DashboardView : UserControl
    {
        public DashboardView()
        {
            InitializeComponent();
        }

        private void BoostRam_Click(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is DashboardViewModel vm)
                vm.BoostRamCommand.Execute(null);
        }

        private void GameMode_Click(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is DashboardViewModel vm)
                vm.GameModeCommand.Execute(null);
        }

        private void NetworkReset_Click(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is DashboardViewModel vm)
                vm.NetworkResetCommand.Execute(null);
        }

        private void FpsBoost_Click(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is DashboardViewModel vm)
                vm.FpsBoostCommand.Execute(null);
        }
    }
}
