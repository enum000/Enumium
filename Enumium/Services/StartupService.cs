using Microsoft.Win32;
using System.IO;

namespace Enumium.Services
{
    public class StartupService
    {
        private static readonly string[] RegistryPaths = {
            @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run",
            @"SOFTWARE\Microsoft\Windows\CurrentVersion\RunOnce",
            @"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Run",
        };

        public static List<Models.StartupItem> GetStartupItems()
        {
            var items = new List<Models.StartupItem>();

            // Registry entries (HKCU)
            foreach (var path in RegistryPaths)
            {
                try
                {
                    using var key = Registry.CurrentUser.OpenSubKey(path);
                    if (key != null)
                    {
                        foreach (var name in key.GetValueNames())
                        {
                            var cmd = key.GetValue(name)?.ToString() ?? "";
                            if (!string.IsNullOrEmpty(cmd))
                            {
                                items.Add(new Models.StartupItem
                                {
                                    Name = name,
                                    Command = cmd,
                                    Location = "Registry (HKCU)",
                                    RegistryPath = $@"HKEY_CURRENT_USER\{path}",
                                    RegistryValueName = name,
                                    IsEnabled = true,
                                    FilePath = ExtractFilePath(cmd),
                                    Impact = EstimateImpact(cmd)
                                });
                            }
                        }
                    }
                }
                catch { }
            }

            // Registry entries (HKLM)
            foreach (var path in RegistryPaths)
            {
                try
                {
                    using var key = Registry.LocalMachine.OpenSubKey(path);
                    if (key != null)
                    {
                        foreach (var name in key.GetValueNames())
                        {
                            var cmd = key.GetValue(name)?.ToString() ?? "";
                            if (!string.IsNullOrEmpty(cmd))
                            {
                                items.Add(new Models.StartupItem
                                {
                                    Name = name,
                                    Command = cmd,
                                    Location = "Registry (HKLM)",
                                    RegistryPath = $@"HKEY_LOCAL_MACHINE\{path}",
                                    RegistryValueName = name,
                                    IsEnabled = true,
                                    FilePath = ExtractFilePath(cmd),
                                    Impact = EstimateImpact(cmd)
                                });
                            }
                        }
                    }
                }
                catch { }
            }

            // Startup Folder items
            var startupPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
            if (Directory.Exists(startupPath))
            {
                foreach (var file in Directory.GetFiles(startupPath))
                {
                    items.Add(new Models.StartupItem
                    {
                        Name = Path.GetFileNameWithoutExtension(file),
                        Command = file,
                        Location = "Startup Folder",
                        FilePath = file,
                        IsEnabled = true,
                        Impact = "Medium"
                    });
                }
            }

            // Check for disabled items
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(
                    @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\StartupApproved\Run");
                if (key != null)
                {
                    foreach (var name in key.GetValueNames())
                    {
                        var data = key.GetValue(name) as byte[];
                        if (data != null && data.Length > 0)
                        {
                            var existing = items.FirstOrDefault(i => i.Name == name);
                            if (existing != null)
                            {
                                existing.IsEnabled = data[0] != 0x03; // 0x03 = disabled
                            }
                        }
                    }
                }
            }
            catch { }

            return items;
        }

        public static bool SetStartupEnabled(Models.StartupItem item, bool enabled)
        {
            try
            {
                if (item.Location == "Startup Folder")
                {
                    // For startup folder items, rename to .disabled
                    if (!enabled && File.Exists(item.FilePath))
                    {
                        File.Move(item.FilePath, item.FilePath + ".disabled");
                        return true;
                    }
                    else if (enabled && File.Exists(item.FilePath + ".disabled"))
                    {
                        File.Move(item.FilePath + ".disabled", item.FilePath);
                        return true;
                    }
                }
                else
                {
                    // For registry items, use StartupApproved
                    var approvedPath = item.Location.Contains("HKLM")
                        ? @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\StartupApproved\Run"
                        : @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\StartupApproved\Run";

                    var root = item.Location.Contains("HKLM") ? Registry.LocalMachine : Registry.CurrentUser;
                    using var key = root.OpenSubKey(approvedPath, true);
                    if (key != null)
                    {
                        byte[] data = new byte[12];
                        data[0] = enabled ? (byte)0x02 : (byte)0x03;
                        key.SetValue(item.RegistryValueName, data, RegistryValueKind.Binary);
                        return true;
                    }
                }
            }
            catch { }
            return false;
        }

        private static string ExtractFilePath(string command)
        {
            if (string.IsNullOrEmpty(command)) return "";
            command = command.Trim();
            if (command.StartsWith('"'))
            {
                var end = command.IndexOf('"', 1);
                if (end > 0) return command.Substring(1, end - 1);
            }
            var space = command.IndexOf(' ');
            return space > 0 ? command[..space] : command;
        }

        private static string EstimateImpact(string command)
        {
            var lower = command.ToLower();
            if (lower.Contains("updater") || lower.Contains("update")) return "Low";
            if (lower.Contains("antivirus") || lower.Contains("security") || lower.Contains("defender")) return "High";
            if (lower.Contains("driver") || lower.Contains("nvidia") || lower.Contains("amd")) return "Medium";
            return "Medium";
        }
    }
}
