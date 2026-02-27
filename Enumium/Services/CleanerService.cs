using System.IO;

namespace Enumium.Services
{
    public class CleanerService
    {
        public static async Task<List<Models.CleanerItem>> ScanAsync(IProgress<string>? progress = null)
        {
            var items = new List<Models.CleanerItem>();
            await Task.Run(() =>
            {
                // Windows Temp
                progress?.Report("Scanning Windows Temp files...");
                ScanDirectory(items, Path.GetTempPath(), "Windows Temp", "\uE74D");

                // Windows Prefetch
                progress?.Report("Scanning Prefetch files...");
                ScanDirectory(items, @"C:\Windows\Prefetch", "Prefetch", "\uE945");

                // Windows Logs
                progress?.Report("Scanning Windows Logs...");
                ScanDirectory(items, @"C:\Windows\Logs", "Windows Logs", "\uE9D5");

                // Windows Crash Dumps
                progress?.Report("Scanning Crash Dumps...");
                ScanDirectory(items, @"C:\Windows\Minidump", "Crash Dumps", "\uEA39");
                var dmpPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CrashDumps");
                ScanDirectory(items, dmpPath, "Application Crash Dumps", "\uEA39");

                // Thumbnail Cache
                progress?.Report("Scanning Thumbnail Cache...");
                var thumbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "Microsoft", "Windows", "Explorer");
                ScanDirectoryPattern(items, thumbPath, "thumbcache_*.db", "Thumbnail Cache", "\uEB9F");

                // Chrome Cache
                progress?.Report("Scanning Chrome Cache...");
                var chromePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "Google", "Chrome", "User Data", "Default", "Cache");
                ScanDirectory(items, chromePath, "Chrome Cache", "\uE774");

                // Edge Cache
                progress?.Report("Scanning Edge Cache...");
                var edgePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "Microsoft", "Edge", "User Data", "Default", "Cache");
                ScanDirectory(items, edgePath, "Edge Cache", "\uE774");

                // Firefox Cache
                progress?.Report("Scanning Firefox Cache...");
                var ffPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "Mozilla", "Firefox", "Profiles");
                if (Directory.Exists(ffPath))
                {
                    foreach (var profile in Directory.GetDirectories(ffPath))
                    {
                        ScanDirectory(items, Path.Combine(profile, "cache2"), "Firefox Cache", "\uE774");
                    }
                }

                // Discord Cache
                progress?.Report("Scanning Discord Cache...");
                var discordPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "discord", "Cache");
                ScanDirectory(items, discordPath, "Discord Cache", "\uE8BD");

                // Steam Logs
                progress?.Report("Scanning Steam files...");
                ScanDirectory(items, @"C:\Program Files (x86)\Steam\logs", "Steam Logs", "\uE7FC");

                // Windows Error Reports
                progress?.Report("Scanning Error Reports...");
                ScanDirectory(items, @"C:\ProgramData\Microsoft\Windows\WER", "Error Reports", "\uE7BA");

                // Recycle Bin
                progress?.Report("Scanning Recycle Bin...");
                ScanRecycleBin(items);

                // Windows Update Cleanup
                progress?.Report("Scanning Windows Update files...");
                ScanDirectory(items, @"C:\Windows\SoftwareDistribution\Download", "Windows Update Cache", "\uE72C");

                progress?.Report("Scan complete!");
            });
            return items;
        }

        private static void ScanDirectory(List<Models.CleanerItem> items, string path, string category, string icon)
        {
            if (!Directory.Exists(path)) return;
            try
            {
                long totalSize = 0;
                int fileCount = 0;
                foreach (var file in Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories))
                {
                    try
                    {
                        var fi = new FileInfo(file);
                        totalSize += fi.Length;
                        fileCount++;
                    }
                    catch { }
                }
                if (fileCount > 0)
                {
                    items.Add(new Models.CleanerItem
                    {
                        Name = category,
                        Category = category,
                        Path = path,
                        Size = totalSize,
                        FileCount = fileCount,
                        Icon = icon,
                        IsSelected = true
                    });
                }
            }
            catch { }
        }

        private static void ScanDirectoryPattern(List<Models.CleanerItem> items, string path, string pattern, string category, string icon)
        {
            if (!Directory.Exists(path)) return;
            try
            {
                long totalSize = 0;
                int fileCount = 0;
                foreach (var file in Directory.EnumerateFiles(path, pattern))
                {
                    try
                    {
                        var fi = new FileInfo(file);
                        totalSize += fi.Length;
                        fileCount++;
                    }
                    catch { }
                }
                if (fileCount > 0)
                {
                    items.Add(new Models.CleanerItem
                    {
                        Name = category,
                        Category = category,
                        Path = path,
                        Size = totalSize,
                        FileCount = fileCount,
                        Icon = icon,
                        IsSelected = true
                    });
                }
            }
            catch { }
        }

        private static void ScanRecycleBin(List<Models.CleanerItem> items)
        {
            try
            {
                long totalSize = 0;
                int fileCount = 0;
                foreach (var drive in DriveInfo.GetDrives().Where(d => d.IsReady))
                {
                    var recyclePath = Path.Combine(drive.RootDirectory.FullName, "$Recycle.Bin");
                    if (Directory.Exists(recyclePath))
                    {
                        try
                        {
                            foreach (var file in Directory.EnumerateFiles(recyclePath, "*", SearchOption.AllDirectories))
                            {
                                try
                                {
                                    var fi = new FileInfo(file);
                                    totalSize += fi.Length;
                                    fileCount++;
                                }
                                catch { }
                            }
                        }
                        catch { }
                    }
                }
                if (fileCount > 0)
                {
                    items.Add(new Models.CleanerItem
                    {
                        Name = "Recycle Bin",
                        Category = "Recycle Bin",
                        Path = "Recycle Bin",
                        Size = totalSize,
                        FileCount = fileCount,
                        Icon = "\uE72C",
                        IsSelected = false // Default to not selected for safety
                    });
                }
            }
            catch { }
        }

        public static async Task<long> CleanAsync(List<Models.CleanerItem> items, IProgress<string>? progress = null)
        {
            long totalFreed = 0;
            await Task.Run(() =>
            {
                foreach (var item in items.Where(i => i.IsSelected))
                {
                    if (item.Name == "Recycle Bin")
                    {
                        // Use SHEmptyRecycleBin for recycle bin
                        progress?.Report($"Emptying Recycle Bin...");
                        try { SHEmptyRecycleBin(IntPtr.Zero, null, 0x07); totalFreed += item.Size; }
                        catch { }
                        continue;
                    }

                    progress?.Report($"Cleaning {item.Name}...");
                    if (Directory.Exists(item.Path))
                    {
                        try
                        {
                            foreach (var file in Directory.EnumerateFiles(item.Path, "*", SearchOption.AllDirectories))
                            {
                                try
                                {
                                    var fi = new FileInfo(file);
                                    long size = fi.Length;
                                    fi.Delete();
                                    totalFreed += size;
                                }
                                catch { }
                            }
                        }
                        catch { }
                    }
                }
                progress?.Report("Cleanup complete!");
            });
            return totalFreed;
        }

        [System.Runtime.InteropServices.DllImport("Shell32.dll", CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
        private static extern uint SHEmptyRecycleBin(IntPtr hwnd, string? pszRootPath, uint dwFlags);
    }
}
