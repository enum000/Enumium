using Microsoft.Win32;
using System.ServiceProcess;

namespace Enumium.Services
{
    public class TweakService
    {
        public static List<Models.TweakItem> GetSystemTweaks()
        {
            return new List<Models.TweakItem>
            {
                // ── System Tweaks ──
                new() { Id = "fast_startup", Name = "Fast Startup", Category = "System",
                    Description = "Enables Windows hybrid boot for faster startup times",
                    Impact = "High", RegistryPath = @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Session Manager\Power",
                    RegistryValue = "HiberbootEnabled", EnabledData = 1, DisabledData = 0 },

                new() { Id = "visual_effects", Name = "Optimize Visual Effects", Category = "System",
                    Description = "Reduces Windows visual effects for better performance",
                    Impact = "Medium", RegistryPath = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\VisualEffects",
                    RegistryValue = "VisualFXSetting", EnabledData = 2, DisabledData = 1 },

                new() { Id = "menu_delay", Name = "Reduce Menu Show Delay", Category = "System",
                    Description = "Sets menu animation delay to 0ms for snappier UI",
                    Impact = "Low", RegistryPath = @"HKEY_CURRENT_USER\Control Panel\Desktop",
                    RegistryValue = "MenuShowDelay", EnabledData = "0", DisabledData = "400" },

                new() { Id = "mouse_hover_time", Name = "Reduce Mouse Hover Time", Category = "System",
                    Description = "Decreases the time before hover events trigger",
                    Impact = "Low", RegistryPath = @"HKEY_CURRENT_USER\Control Panel\Mouse",
                    RegistryValue = "MouseHoverTime", EnabledData = "10", DisabledData = "400" },

                new() { Id = "auto_end_tasks", Name = "Auto End Tasks on Shutdown", Category = "System",
                    Description = "Automatically closes hung applications during shutdown",
                    Impact = "Medium", RegistryPath = @"HKEY_CURRENT_USER\Control Panel\Desktop",
                    RegistryValue = "AutoEndTasks", EnabledData = "1", DisabledData = "0" },

                // ── Memory Optimization ──
                new() { Id = "large_system_cache", Name = "Large System Cache", Category = "Memory",
                    Description = "Optimizes system cache behavior for better memory management",
                    Impact = "Medium", RegistryPath = @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management",
                    RegistryValue = "LargeSystemCache", EnabledData = 1, DisabledData = 0 },

                new() { Id = "disable_paging_executive", Name = "Disable Paging Executive", Category = "Memory",
                    Description = "Keeps drivers and system code in physical memory",
                    Impact = "Medium", RegistryPath = @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management",
                    RegistryValue = "DisablePagingExecutive", EnabledData = 1, DisabledData = 0 },

                new() { Id = "clear_pagefile", Name = "Clear Page File at Shutdown", Category = "Memory",
                    Description = "Clears the page file when Windows shuts down for security",
                    Impact = "Low", RegistryPath = @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management",
                    RegistryValue = "ClearPageFileAtShutdown", EnabledData = 1, DisabledData = 0 },

                // ── Disk Optimization ──
                new() { Id = "ntfs_memory_usage", Name = "NTFS Memory Usage", Category = "Disk",
                    Description = "Increases paged pool memory for NTFS operations",
                    Impact = "Low", RegistryPath = @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\FileSystem",
                    RegistryValue = "NtfsMemoryUsage", EnabledData = 2, DisabledData = 1 },

                new() { Id = "disable_8dot3", Name = "Disable 8.3 Name Creation", Category = "Disk",
                    Description = "Disables legacy short filename generation for NTFS performance",
                    Impact = "Medium", RegistryPath = @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\FileSystem",
                    RegistryValue = "NtfsDisable8dot3NameCreation", EnabledData = 1, DisabledData = 0 },

                new() { Id = "disable_last_access", Name = "Disable Last Access Update", Category = "Disk",
                    Description = "Prevents updating last access timestamp for faster file operations",
                    Impact = "Medium", RegistryPath = @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\FileSystem",
                    RegistryValue = "NtfsDisableLastAccessUpdate", EnabledData = 2147483649u, DisabledData = 2147483648u },

                // ── Gaming Boost ──
                new() { Id = "game_mode", Name = "Windows Game Mode", Category = "Gaming",
                    Description = "Enables Windows Game Mode for optimized gaming performance",
                    Impact = "Medium", RegistryPath = @"HKEY_CURRENT_USER\Software\Microsoft\GameBar",
                    RegistryValue = "AllowAutoGameMode", EnabledData = 1, DisabledData = 0 },

                new() { Id = "gpu_scheduling", Name = "Hardware GPU Scheduling", Category = "Gaming",
                    Description = "Enables hardware-accelerated GPU scheduling for reduced latency",
                    Impact = "High", RegistryPath = @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\GraphicsDrivers",
                    RegistryValue = "HwSchMode", EnabledData = 2, DisabledData = 1 },

                new() { Id = "fullscreen_opt", Name = "Disable Fullscreen Optimizations", Category = "Gaming",
                    Description = "Disables DWM fullscreen optimizations that can cause input lag",
                    Impact = "Medium", RegistryPath = @"HKEY_CURRENT_USER\System\GameConfigStore",
                    RegistryValue = "GameDVR_FSEBehaviorMode", EnabledData = 2, DisabledData = 0 },

                new() { Id = "game_dvr", Name = "Disable Game DVR/Bar", Category = "Gaming",
                    Description = "Disables Game DVR background recording to free up resources",
                    Impact = "High", RegistryPath = @"HKEY_CURRENT_USER\System\GameConfigStore",
                    RegistryValue = "GameDVR_Enabled", EnabledData = 0, DisabledData = 1 },

                new() { Id = "network_throttling", Name = "Disable Network Throttling", Category = "Gaming",
                    Description = "Removes network throttling index for better online gaming",
                    Impact = "Medium", RegistryPath = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile",
                    RegistryValue = "NetworkThrottlingIndex", EnabledData = unchecked((int)0xFFFFFFFF), DisabledData = 10 },

                new() { Id = "priority_separation", Name = "High Priority Foreground", Category = "Gaming",
                    Description = "Gives more CPU time to foreground applications (games)",
                    Impact = "Medium", RegistryPath = @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\PriorityControl",
                    RegistryValue = "Win32PrioritySeparation", EnabledData = 38, DisabledData = 2 },
            };
        }

        public static bool ReadTweakState(Models.TweakItem tweak)
        {
            try
            {
                var val = Registry.GetValue(tweak.RegistryPath, tweak.RegistryValue, null);
                if (val != null && tweak.EnabledData != null)
                {
                    return val.ToString() == tweak.EnabledData.ToString();
                }
            }
            catch { }
            return false;
        }

        public static bool ApplyTweak(Models.TweakItem tweak, bool enable)
        {
            try
            {
                var data = enable ? tweak.EnabledData : tweak.DisabledData;
                if (data == null) return false;

                if (data is int intVal)
                    Registry.SetValue(tweak.RegistryPath, tweak.RegistryValue, intVal, RegistryValueKind.DWord);
                else if (data is uint uintVal)
                    Registry.SetValue(tweak.RegistryPath, tweak.RegistryValue, unchecked((int)uintVal), RegistryValueKind.DWord);
                else if (data is string strVal)
                    Registry.SetValue(tweak.RegistryPath, tweak.RegistryValue, strVal, RegistryValueKind.String);
                else
                    Registry.SetValue(tweak.RegistryPath, tweak.RegistryValue, data);

                return true;
            }
            catch { return false; }
        }

        public static List<Models.ServiceItem> GetServices()
        {
            var items = new List<Models.ServiceItem>();
            try
            {
                var safeToDisable = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                {
                    "DiagTrack", "dmwappushservice", "MapsBroker", "lfsvc", "RetailDemo",
                    "WMPNetworkSvc", "WSearch", "SysMain", "Fax", "XblAuthManager",
                    "XblGameSave", "XboxNetApiSvc", "XboxGipSvc"
                };

                foreach (var svc in ServiceController.GetServices().OrderBy(s => s.DisplayName))
                {
                    try
                    {
                        items.Add(new Models.ServiceItem
                        {
                            Name = svc.ServiceName,
                            DisplayName = svc.DisplayName,
                            Status = svc.Status.ToString(),
                            StartupType = svc.StartType.ToString(),
                            SafeToDisable = safeToDisable.Contains(svc.ServiceName)
                        });
                    }
                    catch { }
                }
            }
            catch { }
            return items;
        }
    }
}
