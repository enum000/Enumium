using System.Windows.Controls;
using System.Windows.Input;
using Enumium.ViewModels;

namespace Enumium.Views
{
    public partial class NetworkView : UserControl
    {
        public NetworkView() { InitializeComponent(); }

        private void FlushDns_Click(object sender, MouseButtonEventArgs e)
        { if (DataContext is NetworkViewModel vm) vm.FlushDnsCommand.Execute(null); }

        private void ResetWinsock_Click(object sender, MouseButtonEventArgs e)
        { if (DataContext is NetworkViewModel vm) vm.ResetWinsockCommand.Execute(null); }

        private void RenewIp_Click(object sender, MouseButtonEventArgs e)
        { if (DataContext is NetworkViewModel vm) vm.RenewIpCommand.Execute(null); }

        private void ResetStack_Click(object sender, MouseButtonEventArgs e)
        { if (DataContext is NetworkViewModel vm) vm.ResetStackCommand.Execute(null); }

        private void DnsPreset_Click(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is NetworkViewModel vm && sender is System.Windows.FrameworkElement el
                && el.DataContext is Models.DnsPreset dns)
                vm.SetDnsCommand.Execute(dns);
        }
    }
}
