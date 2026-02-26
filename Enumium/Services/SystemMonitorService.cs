using System.Diagnostics;
using System.Management;
using System.Runtime.InteropServices;

namespace Enumium.Services
{
    public class SystemMonitorService : IDisposable
    {
        private PerformanceCounter? _cpuCounter;
        private PerformanceCounter? _ramCounter;
        private PerformanceCounter? _diskCounter;
        private readonly System.Timers.Timer _timer;
        private bool _initialized;

        public event Action<double, double, double>? OnMetricsUpdated;

        public double CpuUsage { get; private set; }
        public double RamUsage { get; private set; }
        public double DiskUsage { get; private set; }
        public long TotalRamMB { get; private set; }
        public long UsedRamMB { get; private set; }
        public long FreeRamMB { get; private set; }

        public SystemMonitorService()
        {
            _timer = new System.Timers.Timer(1000);
            _timer.Elapsed += (s, e) => UpdateMetrics();
        }

        public void Start()
        {
            try
            {
                _cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
                _ramCounter = new PerformanceCounter("Memory", "Available MBytes");
                _diskCounter = new PerformanceCounter("PhysicalDisk", "% Disk Time", "_Total");
                _cpuCounter.NextValue();
                _initialized = true;
            }
            catch
            {
                _initialized = false;
            }

            GetTotalRam();
            _timer.Start();
        }

        private void GetTotalRam()
        {
            try
            {
                using var searcher = new ManagementObjectSearcher("SELECT TotalVisibleMemorySize FROM Win32_OperatingSystem");
                foreach (var obj in searcher.Get())
                {
                    TotalRamMB = System.Convert.ToInt64(obj["TotalVisibleMemorySize"]) / 1024;
                }
            }
            catch { TotalRamMB = 8192; }
        }

        private void UpdateMetrics()
        {
            try
            {
                if (_initialized)
                {
                    CpuUsage = Math.Round(_cpuCounter!.NextValue(), 1);
                    double availMB = _ramCounter!.NextValue();
                    FreeRamMB = (long)availMB;
                    UsedRamMB = TotalRamMB - FreeRamMB;
                    RamUsage = TotalRamMB > 0 ? Math.Round((double)UsedRamMB / TotalRamMB * 100, 1) : 0;
                    DiskUsage = Math.Round(Math.Min(_diskCounter!.NextValue(), 100), 1);
                }
                else
                {
                    // Fallback without perf counters
                    var memStatus = new MEMORYSTATUSEX { dwLength = (uint)Marshal.SizeOf<MEMORYSTATUSEX>() };
                    GlobalMemoryStatusEx(ref memStatus);
                    TotalRamMB = (long)(memStatus.ullTotalPhys / 1024 / 1024);
                    FreeRamMB = (long)(memStatus.ullAvailPhys / 1024 / 1024);
                    UsedRamMB = TotalRamMB - FreeRamMB;
                    RamUsage = Math.Round((double)UsedRamMB / TotalRamMB * 100, 1);
                    CpuUsage = 0;
                    DiskUsage = 0;
                }

                OnMetricsUpdated?.Invoke(CpuUsage, RamUsage, DiskUsage);
            }
            catch { }
        }

        public static string GetCpuName()
        {
            try
            {
                using var searcher = new ManagementObjectSearcher("SELECT Name FROM Win32_Processor");
                foreach (var obj in searcher.Get())
                    return obj["Name"]?.ToString()?.Trim() ?? "Unknown CPU";
            }
            catch { }
            return "Unknown CPU";
        }

        public static string GetGpuName()
        {
            try
            {
                using var searcher = new ManagementObjectSearcher("SELECT Name FROM Win32_VideoController");
                foreach (var obj in searcher.Get())
                    return obj["Name"]?.ToString()?.Trim() ?? "Unknown GPU";
            }
            catch { }
            return "Unknown GPU";
        }

        public static string GetOsVersion()
        {
            return $"Windows {Environment.OSVersion.Version.Major}.{Environment.OSVersion.Version.Build}";
        }

        public static TimeSpan GetUptime()
        {
            return TimeSpan.FromMilliseconds(Environment.TickCount64);
        }

        public static List<Models.ProcessInfo> GetTopProcesses(int count = 5)
        {
            var processes = new List<Models.ProcessInfo>();
            try
            {
                foreach (var p in Process.GetProcesses()
                    .Where(p => p.WorkingSet64 > 0)
                    .OrderByDescending(p => p.WorkingSet64)
                    .Take(count))
                {
                    try
                    {
                        processes.Add(new Models.ProcessInfo
                        {
                            Pid = p.Id,
                            Name = p.ProcessName,
                            MemoryMB = p.WorkingSet64 / (1024 * 1024),
                            Status = p.Responding ? "Running" : "Not Responding"
                        });
                    }
                    catch { }
                }
            }
            catch { }
            return processes;
        }

        public static void KillProcess(int pid)
        {
            try
            {
                var proc = Process.GetProcessById(pid);
                proc.Kill();
            }
            catch { }
        }

        public static void BoostRam()
        {
            try
            {
                foreach (var proc in Process.GetProcesses())
                {
                    try
                    {
                        Helpers.NativeMethods.EmptyWorkingSet(proc.Handle);
                    }
                    catch { }
                }
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            catch { }
        }

        public void Dispose()
        {
            _timer.Stop();
            _timer.Dispose();
            _cpuCounter?.Dispose();
            _ramCounter?.Dispose();
            _diskCounter?.Dispose();
        }

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GlobalMemoryStatusEx(ref MEMORYSTATUSEX lpBuffer);

        [StructLayout(LayoutKind.Sequential)]
        private struct MEMORYSTATUSEX
        {
            public uint dwLength;
            public uint dwMemoryLoad;
            public ulong ullTotalPhys;
            public ulong ullAvailPhys;
            public ulong ullTotalPageFile;
            public ulong ullAvailPageFile;
            public ulong ullTotalVirtual;
            public ulong ullAvailVirtual;
            public ulong ullAvailExtendedVirtual;
        }
    }
}
