using Enumium.Helpers;
using Enumium.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Management;
using System.IO;

namespace Enumium.ViewModels
{
    public class ToolsViewModel : ViewModelBase
    {
        // ── System Info ──
        private string _systemInfo = "";
        public string SystemInfo { get => _systemInfo; set => SetProperty(ref _systemInfo, value); }

        // ── Process Manager ──
        public ObservableCollection<Models.ProcessInfo> Processes { get; } = new();

        // ── Benchmark ──
        private string _benchmarkResult = "";
        public string BenchmarkResult { get => _benchmarkResult; set => SetProperty(ref _benchmarkResult, value); }

        private bool _isBenchmarking;
        public bool IsBenchmarking { get => _isBenchmarking; set => SetProperty(ref _isBenchmarking, value); }

        private string _statusText = "";
        public string StatusText { get => _statusText; set => SetProperty(ref _statusText, value); }

        private int _selectedTool;
        public int SelectedTool { get => _selectedTool; set => SetProperty(ref _selectedTool, value); }

        public RelayCommand SelectToolCommand { get; }
        public RelayCommand RefreshProcessesCommand { get; }
        public RelayCommand KillProcessCommand { get; }
        public RelayCommand RunBenchmarkCommand { get; }
        public RelayCommand LoadSystemInfoCommand { get; }

        public ToolsViewModel()
        {
            SelectToolCommand = new RelayCommand(p => { if (p is string s && int.TryParse(s, out int i)) SelectedTool = i; });
            RefreshProcessesCommand = new RelayCommand(_ => RefreshProcesses());
            KillProcessCommand = new RelayCommand(p => { if (p is int pid) { SystemMonitorService.KillProcess(pid); RefreshProcesses(); } });
            RunBenchmarkCommand = new RelayCommand(async _ => await RunBenchmark());
            LoadSystemInfoCommand = new RelayCommand(_ => LoadSystemInfo());

            LoadSystemInfo();
            RefreshProcesses();
        }

        private void LoadSystemInfo()
        {
            var info = new System.Text.StringBuilder();
            info.AppendLine("═══════════════════════════════════════");
            info.AppendLine("           SYSTEM INFORMATION          ");
            info.AppendLine("═══════════════════════════════════════");
            info.AppendLine();

            // OS
            info.AppendLine($"  OS:          {Environment.OSVersion}");
            info.AppendLine($"  Computer:    {Environment.MachineName}");
            info.AppendLine($"  User:        {Environment.UserName}");
            info.AppendLine($"  64-Bit OS:   {Environment.Is64BitOperatingSystem}");
            info.AppendLine($"  Processors:  {Environment.ProcessorCount} logical cores");
            info.AppendLine();

            // CPU
            try
            {
                using var cpuSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_Processor");
                foreach (var obj in cpuSearcher.Get())
                {
                    info.AppendLine($"  CPU:         {obj["Name"]}");
                    info.AppendLine($"  CPU Cores:   {obj["NumberOfCores"]}");
                    info.AppendLine($"  CPU Speed:   {obj["MaxClockSpeed"]} MHz");
                }
            }
            catch { info.AppendLine("  CPU:         Unable to query"); }

            info.AppendLine();

            // GPU
            try
            {
                using var gpuSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_VideoController");
                foreach (var obj in gpuSearcher.Get())
                {
                    info.AppendLine($"  GPU:         {obj["Name"]}");
                    var ram = Convert.ToInt64(obj["AdapterRAM"] ?? 0);
                    if (ram > 0) info.AppendLine($"  VRAM:        {ram / 1024 / 1024} MB");
                    info.AppendLine($"  Resolution:  {obj["CurrentHorizontalResolution"]}x{obj["CurrentVerticalResolution"]}");
                }
            }
            catch { info.AppendLine("  GPU:         Unable to query"); }

            info.AppendLine();

            // RAM
            try
            {
                using var memSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMemory");
                long totalBytes = 0;
                foreach (var obj in memSearcher.Get())
                {
                    totalBytes += Convert.ToInt64(obj["Capacity"]);
                }
                info.AppendLine($"  Total RAM:   {totalBytes / 1024 / 1024 / 1024} GB");
            }
            catch { }

            info.AppendLine();

            // Disks
            foreach (var drive in DriveInfo.GetDrives().Where(d => d.IsReady))
            {
                info.AppendLine($"  Drive {drive.Name}  {drive.DriveFormat}  " +
                    $"{drive.AvailableFreeSpace / 1024 / 1024 / 1024} GB free / " +
                    $"{drive.TotalSize / 1024 / 1024 / 1024} GB total");
            }

            SystemInfo = info.ToString();
        }

        private void RefreshProcesses()
        {
            Processes.Clear();
            try
            {
                foreach (var p in Process.GetProcesses()
                    .Where(p => p.WorkingSet64 > 0)
                    .OrderByDescending(p => p.WorkingSet64)
                    .Take(50))
                {
                    try
                    {
                        Processes.Add(new Models.ProcessInfo
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
        }

        private async Task RunBenchmark()
        {
            IsBenchmarking = true;
            BenchmarkResult = "Running CPU benchmark...";

            await Task.Run(() =>
            {
                var results = new System.Text.StringBuilder();
                results.AppendLine("═══ ENUMIUM BENCHMARK RESULTS ═══\n");

                // CPU Benchmark
                var sw = Stopwatch.StartNew();
                double sum = 0;
                for (int i = 0; i < 50_000_000; i++)
                    sum += Math.Sqrt(i) * Math.Sin(i);
                sw.Stop();
                double cpuScore = 10000.0 / sw.ElapsedMilliseconds * 1000;
                results.AppendLine($"  CPU Score:    {cpuScore:F0} pts ({sw.ElapsedMilliseconds} ms)");

                // Memory Benchmark
                BenchmarkResult = "Running memory benchmark...";
                sw.Restart();
                var arr = new byte[256 * 1024 * 1024]; // 256MB
                for (int i = 0; i < arr.Length; i += 4096)
                    arr[i] = 0xFF;
                sw.Stop();
                double memScore = 256.0 / sw.Elapsed.TotalSeconds * 1024; // MB/s
                results.AppendLine($"  Memory:       {memScore:F0} MB/s ({sw.ElapsedMilliseconds} ms)");

                // Disk Benchmark
                BenchmarkResult = "Running disk benchmark...";
                var tempFile = Path.GetTempFileName();
                sw.Restart();
                var data = new byte[64 * 1024 * 1024]; // 64MB
                new Random().NextBytes(data);
                File.WriteAllBytes(tempFile, data);
                sw.Stop();
                double writeSpeed = 64.0 / sw.Elapsed.TotalSeconds;
                results.AppendLine($"  Disk Write:   {writeSpeed:F0} MB/s");

                sw.Restart();
                _ = File.ReadAllBytes(tempFile);
                sw.Stop();
                double readSpeed = 64.0 / sw.Elapsed.TotalSeconds;
                results.AppendLine($"  Disk Read:    {readSpeed:F0} MB/s");

                try { File.Delete(tempFile); } catch { }

                double total = (cpuScore + memScore + writeSpeed + readSpeed) / 4;
                results.AppendLine($"\n  Overall:      {total:F0} pts");
                results.AppendLine($"\n  Date: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");

                BenchmarkResult = results.ToString();
                ReportService.LogOptimization("Benchmark", "Tools", $"Overall score: {total:F0}", true);
            });

            IsBenchmarking = false;
        }
    }
}
