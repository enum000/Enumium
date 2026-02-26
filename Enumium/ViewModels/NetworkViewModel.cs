using Enumium.Helpers;
using Enumium.Services;
using System.Collections.ObjectModel;

namespace Enumium.ViewModels
{
    public class NetworkViewModel : ViewModelBase
    {
        public ObservableCollection<Models.NetworkAdapterInfo> Adapters { get; } = new();
        public ObservableCollection<Models.DnsPreset> DnsPresets { get; } = new();

        private string _googlePing = "—";
        public string GooglePing { get => _googlePing; set => SetProperty(ref _googlePing, value); }

        private string _cloudflarePing = "—";
        public string CloudflarePing { get => _cloudflarePing; set => SetProperty(ref _cloudflarePing, value); }

        private string _statusText = "";
        public string StatusText { get => _statusText; set => SetProperty(ref _statusText, value); }

        private bool _isBusy;
        public bool IsBusy { get => _isBusy; set => SetProperty(ref _isBusy, value); }

        public RelayCommand RefreshCommand { get; }
        public RelayCommand PingTestCommand { get; }
        public RelayCommand FlushDnsCommand { get; }
        public RelayCommand ResetWinsockCommand { get; }
        public RelayCommand RenewIpCommand { get; }
        public RelayCommand ResetStackCommand { get; }
        public RelayCommand SetDnsCommand { get; }
        public RelayCommand ApplyTcpOptCommand { get; }

        public NetworkViewModel()
        {
            RefreshCommand = new RelayCommand(_ => LoadAdapters());
            PingTestCommand = new RelayCommand(async _ => await RunPingTest());
            FlushDnsCommand = new RelayCommand(async _ => await DoFlushDns());
            ResetWinsockCommand = new RelayCommand(async _ => await DoResetWinsock());
            RenewIpCommand = new RelayCommand(async _ => await DoRenewIp());
            ResetStackCommand = new RelayCommand(async _ => await DoResetStack());
            SetDnsCommand = new RelayCommand(async p => { if (p is Models.DnsPreset dns) await DoSetDns(dns); });
            ApplyTcpOptCommand = new RelayCommand(_ => DoApplyTcpOpt());

            foreach (var dns in NetworkService.GetDnsPresets())
                DnsPresets.Add(dns);

            LoadAdapters();
        }

        private void LoadAdapters()
        {
            Adapters.Clear();
            foreach (var a in NetworkService.GetAdapters())
                Adapters.Add(a);
            StatusText = $"Found {Adapters.Count} network adapters";
        }

        private async Task RunPingTest()
        {
            IsBusy = true;
            GooglePing = "Testing...";
            CloudflarePing = "Testing...";

            var gPing = await NetworkService.PingHostAsync("8.8.8.8");
            GooglePing = gPing >= 0 ? $"{gPing} ms" : "Failed";

            var cPing = await NetworkService.PingHostAsync("1.1.1.1");
            CloudflarePing = cPing >= 0 ? $"{cPing} ms" : "Failed";
            IsBusy = false;
        }

        private async Task DoFlushDns()
        {
            IsBusy = true;
            await NetworkService.FlushDnsAsync();
            StatusText = "✅ DNS cache flushed successfully";
            ReportService.LogOptimization("Flush DNS", "Network", "DNS cache cleared", true);
            IsBusy = false;
        }

        private async Task DoResetWinsock()
        {
            IsBusy = true;
            await NetworkService.ResetWinsockAsync();
            StatusText = "✅ Winsock reset successfully (restart required)";
            ReportService.LogOptimization("Reset Winsock", "Network", "Winsock catalog reset", true);
            IsBusy = false;
        }

        private async Task DoRenewIp()
        {
            IsBusy = true;
            await NetworkService.RenewIpAsync();
            StatusText = "✅ IP renewed successfully";
            ReportService.LogOptimization("Renew IP", "Network", "IP address renewed", true);
            LoadAdapters();
            IsBusy = false;
        }

        private async Task DoResetStack()
        {
            IsBusy = true;
            await NetworkService.ResetNetworkStackAsync();
            StatusText = "✅ Full network stack reset (restart required)";
            ReportService.LogOptimization("Reset Network Stack", "Network", "Complete network stack reset", true);
            IsBusy = false;
        }

        private async Task DoSetDns(Models.DnsPreset dns)
        {
            IsBusy = true;
            bool ok = await NetworkService.SetDnsAsync(dns.Primary, dns.Secondary);
            StatusText = ok ? $"✅ DNS set to {dns.Name} ({dns.Primary} / {dns.Secondary})"
                           : "❌ Failed to set DNS";
            if (ok) ReportService.LogOptimization("Set DNS", "Network", $"Changed to {dns.Name}", true);
            LoadAdapters();
            IsBusy = false;
        }

        private void DoApplyTcpOpt()
        {
            NetworkService.ApplyTcpOptimizations();
            StatusText = "✅ TCP/IP optimizations applied";
            ReportService.LogOptimization("TCP Optimizations", "Network", "Applied TCP/IP tuning", true);
        }
    }
}
