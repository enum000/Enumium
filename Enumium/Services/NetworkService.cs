using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using Microsoft.Win32;

namespace Enumium.Services
{
    public class NetworkService
    {
        public static List<Models.NetworkAdapterInfo> GetAdapters()
        {
            var adapters = new List<Models.NetworkAdapterInfo>();
            try
            {
                foreach (var nic in NetworkInterface.GetAllNetworkInterfaces()
                    .Where(n => n.NetworkInterfaceType != NetworkInterfaceType.Loopback))
                {
                    var props = nic.GetIPProperties();
                    var ipv4 = props.UnicastAddresses
                        .FirstOrDefault(a => a.Address.AddressFamily == AddressFamily.InterNetwork);

                    adapters.Add(new Models.NetworkAdapterInfo
                    {
                        Name = nic.Name,
                        Description = nic.Description,
                        Type = nic.NetworkInterfaceType.ToString(),
                        Status = nic.OperationalStatus.ToString(),
                        IpAddress = ipv4?.Address.ToString() ?? "N/A",
                        SubnetMask = ipv4?.IPv4Mask?.ToString() ?? "N/A",
                        Gateway = props.GatewayAddresses.FirstOrDefault()?.Address.ToString() ?? "N/A",
                        DnsServers = string.Join(", ", props.DnsAddresses.Select(d => d.ToString())),
                        MacAddress = nic.GetPhysicalAddress().ToString(),
                        Speed = nic.Speed,
                        BytesSent = nic.GetIPStatistics().BytesSent,
                        BytesReceived = nic.GetIPStatistics().BytesReceived
                    });
                }
            }
            catch { }
            return adapters;
        }

        public static async Task<long> PingHostAsync(string host)
        {
            try
            {
                using var ping = new Ping();
                var reply = await ping.SendPingAsync(host, 3000);
                return reply.Status == IPStatus.Success ? reply.RoundtripTime : -1;
            }
            catch { return -1; }
        }

        public static async Task<string> FlushDnsAsync()
        {
            return await RunNetshCommand("ipconfig /flushdns");
        }

        public static async Task<string> ResetWinsockAsync()
        {
            return await RunNetshCommand("netsh winsock reset");
        }

        public static async Task<string> RenewIpAsync()
        {
            await RunNetshCommand("ipconfig /release");
            return await RunNetshCommand("ipconfig /renew");
        }

        public static async Task<string> ResetNetworkStackAsync()
        {
            var results = new List<string>();
            results.Add(await RunNetshCommand("netsh winsock reset"));
            results.Add(await RunNetshCommand("netsh int ip reset"));
            results.Add(await RunNetshCommand("ipconfig /flushdns"));
            results.Add(await RunNetshCommand("ipconfig /release"));
            results.Add(await RunNetshCommand("ipconfig /renew"));
            return string.Join("\n", results);
        }

        public static List<Models.DnsPreset> GetDnsPresets()
        {
            return new List<Models.DnsPreset>
            {
                new() { Name = "Google DNS", Primary = "8.8.8.8", Secondary = "8.8.4.4" },
                new() { Name = "Cloudflare DNS", Primary = "1.1.1.1", Secondary = "1.0.0.1" },
                new() { Name = "OpenDNS", Primary = "208.67.222.222", Secondary = "208.67.220.220" },
                new() { Name = "Quad9", Primary = "9.9.9.9", Secondary = "149.112.112.112" },
                new() { Name = "AdGuard DNS", Primary = "94.140.14.14", Secondary = "94.140.15.15" }
            };
        }

        public static async Task<bool> SetDnsAsync(string primary, string secondary)
        {
            try
            {
                // Get the active adapter name
                var activeAdapter = NetworkInterface.GetAllNetworkInterfaces()
                    .FirstOrDefault(n => n.OperationalStatus == OperationalStatus.Up &&
                        n.NetworkInterfaceType != NetworkInterfaceType.Loopback);

                if (activeAdapter == null) return false;

                var name = activeAdapter.Name;
                await RunNetshCommand($"netsh interface ip set dns name=\"{name}\" static {primary}");
                await RunNetshCommand($"netsh interface ip add dns name=\"{name}\" {secondary} index=2");
                return true;
            }
            catch { return false; }
        }

        // TCP/IP Optimizations
        public static void ApplyTcpOptimizations()
        {
            try
            {
                // Auto-tuning level
                RunNetshCommand("netsh int tcp set global autotuninglevel=normal").Wait();
                // RSS
                RunNetshCommand("netsh int tcp set global rss=enabled").Wait();
                // TCP Chimney
                RunNetshCommand("netsh int tcp set global chimney=enabled").Wait();
                // Direct Cache Access
                RunNetshCommand("netsh int tcp set global dca=enabled").Wait();
                // ECN
                RunNetshCommand("netsh int tcp set global ecncapability=enabled").Wait();

                // Registry tweaks
                Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile",
                    "NetworkThrottlingIndex", unchecked((int)0xFFFFFFFF), RegistryValueKind.DWord);
            }
            catch { }
        }

        private static async Task<string> RunNetshCommand(string command)
        {
            try
            {
                var parts = command.Split(' ', 2);
                var psi = new ProcessStartInfo
                {
                    FileName = parts[0],
                    Arguments = parts.Length > 1 ? parts[1] : "",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    Verb = "runas"
                };
                using var process = Process.Start(psi);
                if (process != null)
                {
                    string output = await process.StandardOutput.ReadToEndAsync();
                    await process.WaitForExitAsync();
                    return output.Trim();
                }
            }
            catch (Exception ex) { return ex.Message; }
            return "Command failed";
        }
    }
}
