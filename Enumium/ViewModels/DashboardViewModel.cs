using Enumium.Helpers;
using Enumium.Services;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Threading;

namespace Enumium.ViewModels
{
    public class RecommendedAction : ViewModelBase
    {
        public string Icon { get; set; } = "";
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string Severity { get; set; } = "Info"; // Info, Warning, Critical
        public string ActionLabel { get; set; } = "Fix";
        public Action? OnApply { get; set; }

        private bool _isFixed;
        public bool IsFixed { get => _isFixed; set => SetProperty(ref _isFixed, value); }
    }

    public class DashboardViewModel : ViewModelBase, IDisposable
    {
        private readonly SystemMonitorService _monitor;
        private readonly DispatcherTimer _uiTimer;

        // ── Metrics ──
        private double _cpuUsage;
        public double CpuUsage { get => _cpuUsage; set => SetProperty(ref _cpuUsage, value); }

        private double _ramUsage;
        public double RamUsage { get => _ramUsage; set => SetProperty(ref _ramUsage, value); }

        private double _diskUsage;
        public double DiskUsage { get => _diskUsage; set => SetProperty(ref _diskUsage, value); }

        private string _ramText = "0 / 0 GB";
        public string RamText { get => _ramText; set => SetProperty(ref _ramText, value); }

        private double _healthScore = 85;
        public double HealthScore { get => _healthScore; set => SetProperty(ref _healthScore, value); }

        private string _cpuName = "";
        public string CpuName { get => _cpuName; set => SetProperty(ref _cpuName, value); }

        private string _gpuName = "";
        public string GpuName { get => _gpuName; set => SetProperty(ref _gpuName, value); }

        private string _osVersion = "";
        public string OsVersion { get => _osVersion; set => SetProperty(ref _osVersion, value); }

        private string _uptime = "0:00:00";
        public string Uptime { get => _uptime; set => SetProperty(ref _uptime, value); }

        private int _processCount;
        public int ProcessCount { get => _processCount; set => SetProperty(ref _processCount, value); }

        private string _statusText = "";
        public string StatusText { get => _statusText; set => SetProperty(ref _statusText, value); }

        // ── Localized Labels ──
        public string TitleText => LocalizationService.Get("dash_title");
        public string QuickActionsText => LocalizationService.Get("dash_quick_actions");
        public string TopProcessesText => LocalizationService.Get("dash_top_processes");
        public string RecommendedText => LocalizationService.Get("dash_recommended");
        public string BoostRamText => LocalizationService.Get("dash_boost_ram");
        public string BoostRamDesc => LocalizationService.Get("dash_boost_ram_desc");
        public string GameModeText => LocalizationService.Get("dash_game_mode");
        public string GameModeDesc => LocalizationService.Get("dash_game_mode_desc");
        public string NetworkResetText => LocalizationService.Get("dash_network_reset");
        public string NetworkResetDesc => LocalizationService.Get("dash_network_reset_desc");
        public string HealthText => LocalizationService.Get("dash_health");
        public string EndText => LocalizationService.Get("dash_end");

        // ── Graph Data ──
        public ObservableCollection<double> CpuHistory { get; } = new();
        public ObservableCollection<double> RamHistory { get; } = new();

        // ── Top Processes ──
        public ObservableCollection<Models.ProcessInfo> TopProcesses { get; } = new();

        // ── Recommended Actions ──
        public ObservableCollection<RecommendedAction> RecommendedActions { get; } = new();

        // ── Commands ──
        public RelayCommand BoostRamCommand { get; }
        public RelayCommand KillProcessCommand { get; }
        public RelayCommand GameModeCommand { get; }
        public RelayCommand NetworkResetCommand { get; }
        public RelayCommand ApplyRecommendationCommand { get; }
        public RelayCommand FpsBoostCommand { get; }

        private bool _isBoostingRam;
        public bool IsBoostingRam { get => _isBoostingRam; set => SetProperty(ref _isBoostingRam, value); }

        public DashboardViewModel()
        {
            _monitor = new SystemMonitorService();

            StatusText = LocalizationService.Get("dash_status_good");

            BoostRamCommand = new RelayCommand(_ => BoostRam());
            KillProcessCommand = new RelayCommand(p => { if (p is int pid) SystemMonitorService.KillProcess(pid); RefreshProcesses(); });
            GameModeCommand = new RelayCommand(_ => ToggleGameMode());
            NetworkResetCommand = new RelayCommand(async _ => await ResetNetwork());
            FpsBoostCommand = new RelayCommand(_ => ApplyFpsBoost());
            ApplyRecommendationCommand = new RelayCommand(p =>
            {
                if (p is RecommendedAction rec && !rec.IsFixed)
                {
                    rec.OnApply?.Invoke();
                    rec.IsFixed = true;
                    ReportService.LogOptimization("Auto-Fix", "Dashboard", rec.Title, true);
                }
            });

            for (int i = 0; i < 60; i++)
            {
                CpuHistory.Add(0);
                RamHistory.Add(0);
            }

            CpuName = SystemMonitorService.GetCpuName();
            GpuName = SystemMonitorService.GetGpuName();
            OsVersion = SystemMonitorService.GetOsVersion();

            _monitor.OnMetricsUpdated += (cpu, ram, disk) =>
            {
                Application.Current?.Dispatcher.BeginInvoke(() =>
                {
                    CpuUsage = cpu;
                    RamUsage = ram;
                    DiskUsage = disk;
                    RamText = $"{_monitor.UsedRamMB / 1024.0:F1} / {_monitor.TotalRamMB / 1024.0:F1} GB";

                    CpuHistory.Add(cpu);
                    if (CpuHistory.Count > 60) CpuHistory.RemoveAt(0);
                    RamHistory.Add(ram);
                    if (RamHistory.Count > 60) RamHistory.RemoveAt(0);

                    UpdateHealthScore();
                });
            };

            _monitor.Start();

            _uiTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(3) };
            _uiTimer.Tick += (s, e) =>
            {
                var up = SystemMonitorService.GetUptime();
                Uptime = $"{(int)up.TotalHours}:{up.Minutes:D2}:{up.Seconds:D2}";
                ProcessCount = System.Diagnostics.Process.GetProcesses().Length;
                RefreshProcesses();
            };
            _uiTimer.Start();

            ProcessCount = System.Diagnostics.Process.GetProcesses().Length;
            RefreshProcesses();
            var uptime = SystemMonitorService.GetUptime();
            Uptime = $"{(int)uptime.TotalHours}:{uptime.Minutes:D2}:{uptime.Seconds:D2}";

            // Build recommended actions
            BuildRecommendations();
        }

        private void BuildRecommendations()
        {
            RecommendedActions.Clear();

            // Check RAM usage
            if (RamUsage > 70 || _monitor.UsedRamMB > 0)
            {
                RecommendedActions.Add(new RecommendedAction
                {
                    Icon = "\uE7F4",
                    Title = LocalizationService.CurrentLanguage == "ru" ? "Освободить оперативную память" : "Free Up RAM",
                    Description = LocalizationService.CurrentLanguage == "ru"
                        ? "Очистить рабочие наборы всех процессов для освобождения памяти"
                        : "Empty working sets of all processes to free up memory",
                    Severity = "Warning",
                    ActionLabel = LocalizationService.CurrentLanguage == "ru" ? "Оптимизировать" : "Optimize",
                    OnApply = () => { Task.Run(() => SystemMonitorService.BoostRam()); }
                });
            }

            // DNS flush recommendation
            RecommendedActions.Add(new RecommendedAction
            {
                Icon = "\uE774",
                Title = LocalizationService.CurrentLanguage == "ru" ? "Очистить DNS кэш" : "Flush DNS Cache",
                Description = LocalizationService.CurrentLanguage == "ru"
                    ? "Удалить устаревшие записи DNS для улучшения сетевого соединения"
                    : "Clear stale DNS entries to improve network connectivity",
                Severity = "Info",
                ActionLabel = LocalizationService.CurrentLanguage == "ru" ? "Очистить" : "Flush",
                OnApply = () => { _ = NetworkService.FlushDnsAsync(); }
            });

            // Startup optimization
            RecommendedActions.Add(new RecommendedAction
            {
                Icon = "\uE7B5",
                Title = LocalizationService.CurrentLanguage == "ru" ? "Оптимизировать автозагрузку" : "Optimize Startup Programs",
                Description = LocalizationService.CurrentLanguage == "ru"
                    ? "Проверьте программы автозагрузки и отключите ненужные"
                    : "Review startup programs and disable unnecessary ones to speed up boot time",
                Severity = "Info",
                ActionLabel = LocalizationService.CurrentLanguage == "ru" ? "Проверить" : "Review"
            });

            // Privacy recommendation
            RecommendedActions.Add(new RecommendedAction
            {
                Icon = "\uE72E",
                Title = LocalizationService.CurrentLanguage == "ru" ? "Усилить защиту приватности" : "Enhance Privacy Protection",
                Description = LocalizationService.CurrentLanguage == "ru"
                    ? "Отключить телеметрию Windows и рекламный идентификатор"
                    : "Disable Windows telemetry and advertising ID for better privacy",
                Severity = "Warning",
                ActionLabel = LocalizationService.CurrentLanguage == "ru" ? "Защитить" : "Protect",
                OnApply = () =>
                {
                    var items = PrivacyService.GetPrivacyItems();
                    foreach (var item in items.Take(5))
                        PrivacyService.ApplyPrivacy(item, true);
                }
            });

            // Temp files
            RecommendedActions.Add(new RecommendedAction
            {
                Icon = "\uE74D",
                Title = LocalizationService.CurrentLanguage == "ru" ? "Очистить временные файлы" : "Clean Temporary Files",
                Description = LocalizationService.CurrentLanguage == "ru"
                    ? "Удалите накопленные временные файлы для освобождения места на диске"
                    : "Remove accumulated temporary files to free up disk space",
                Severity = "Info",
                ActionLabel = LocalizationService.CurrentLanguage == "ru" ? "Очистить" : "Clean"
            });

            // TCP Optimization
            RecommendedActions.Add(new RecommendedAction
            {
                Icon = "\uE945",
                Title = LocalizationService.CurrentLanguage == "ru" ? "Оптимизировать TCP/IP" : "Optimize TCP/IP Stack",
                Description = LocalizationService.CurrentLanguage == "ru"
                    ? "Применить сетевые оптимизации для уменьшения задержки"
                    : "Apply network optimizations for reduced latency and improved throughput",
                Severity = "Info",
                ActionLabel = LocalizationService.CurrentLanguage == "ru" ? "Оптимизировать" : "Optimize",
                OnApply = () => { NetworkService.ApplyTcpOptimizations(); }
            });

            // FPS Boost
            RecommendedActions.Add(new RecommendedAction
            {
                Icon = "\uE7FC",
                Title = LocalizationService.CurrentLanguage == "ru" ? "Максимальный FPS (один клик)" : "Maximum FPS Boost (One-Click)",
                Description = LocalizationService.CurrentLanguage == "ru"
                    ? "Применить все оптимизации для максимальной производительности в играх"
                    : "Apply all gaming optimizations for maximum frame rate performance",
                Severity = "Critical",
                ActionLabel = LocalizationService.CurrentLanguage == "ru" ? "Буст" : "Boost",
                OnApply = () => { ApplyFpsBoost(); }
            });
        }

        private void UpdateHealthScore()
        {
            double score = 100;
            if (CpuUsage > 80) score -= 20;
            else if (CpuUsage > 50) score -= 10;

            if (RamUsage > 85) score -= 25;
            else if (RamUsage > 60) score -= 10;

            if (DiskUsage > 80) score -= 15;
            else if (DiskUsage > 50) score -= 5;

            HealthScore = Math.Max(0, Math.Min(100, score));
            StatusText = HealthScore >= 75 ? LocalizationService.Get("dash_status_good") :
                         HealthScore >= 50 ? LocalizationService.Get("dash_status_moderate") :
                         LocalizationService.Get("dash_status_heavy");
        }

        private void RefreshProcesses()
        {
            var procs = SystemMonitorService.GetTopProcesses(5);
            TopProcesses.Clear();
            foreach (var p in procs) TopProcesses.Add(p);
        }

        private async void BoostRam()
        {
            IsBoostingRam = true;
            await Task.Run(() => SystemMonitorService.BoostRam());
            ReportService.LogOptimization("RAM Boost", "Memory", "Emptied working sets of all processes", true);
            IsBoostingRam = false;
        }

        private void ToggleGameMode()
        {
            var tweak = new Models.TweakItem
            {
                RegistryPath = @"HKEY_CURRENT_USER\Software\Microsoft\GameBar",
                RegistryValue = "AllowAutoGameMode",
                EnabledData = 1,
                DisabledData = 0
            };
            bool current = TweakService.ReadTweakState(tweak);
            TweakService.ApplyTweak(tweak, !current);
            ReportService.LogOptimization("Game Mode", "Gaming", current ? "Disabled" : "Enabled", true);
        }

        private async Task ResetNetwork()
        {
            await NetworkService.FlushDnsAsync();
            ReportService.LogOptimization("Network Reset", "Network", "Flushed DNS cache", true);
        }

        private void ApplyFpsBoost()
        {
            var tweaks = TweakService.GetSystemTweaks()
                .Where(t => t.Category == "FPS Boost" || t.Category == "Gaming")
                .ToList();

            int applied = 0;
            foreach (var tweak in tweaks)
            {
                if (TweakService.ApplyTweak(tweak, true))
                {
                    applied++;
                    ReportService.LogOptimization("FPS Boost", "Gaming", tweak.Name, true);
                }
            }

            // Also boost RAM
            Task.Run(() => SystemMonitorService.BoostRam());
        }

        public void Dispose()
        {
            _monitor.Dispose();
            _uiTimer.Stop();
        }
    }
}
