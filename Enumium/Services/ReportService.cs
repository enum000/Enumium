using System.IO;
using System.Text.Json;

namespace Enumium.Services
{
    public class ReportService
    {
        private static readonly string DataDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Enumium");
        private static readonly string HistoryFile = Path.Combine(DataDir, "history.json");

        public static void EnsureDataDir()
        {
            if (!Directory.Exists(DataDir))
                Directory.CreateDirectory(DataDir);
        }

        public static void LogOptimization(string action, string category, string details, bool success)
        {
            EnsureDataDir();
            var history = LoadHistory();
            history.Add(new Models.OptimizationRecord
            {
                Timestamp = DateTime.Now,
                Action = action,
                Category = category,
                Details = details,
                Success = success
            });
            SaveHistory(history);
        }

        public static List<Models.OptimizationRecord> LoadHistory()
        {
            try
            {
                if (File.Exists(HistoryFile))
                {
                    var json = File.ReadAllText(HistoryFile);
                    return JsonSerializer.Deserialize<List<Models.OptimizationRecord>>(json)
                        ?? new List<Models.OptimizationRecord>();
                }
            }
            catch { }
            return new List<Models.OptimizationRecord>();
        }

        private static void SaveHistory(List<Models.OptimizationRecord> history)
        {
            try
            {
                var json = JsonSerializer.Serialize(history, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(HistoryFile, json);
            }
            catch { }
        }

        public static void ClearHistory()
        {
            try
            {
                if (File.Exists(HistoryFile))
                    File.Delete(HistoryFile);
            }
            catch { }
        }

        public static string ExportToCsv(List<Models.OptimizationRecord> records)
        {
            var csv = "Timestamp,Action,Category,Details,Success\n";
            foreach (var r in records)
            {
                csv += $"\"{r.Timestamp:yyyy-MM-dd HH:mm:ss}\",\"{r.Action}\",\"{r.Category}\",\"{r.Details}\",{r.Success}\n";
            }
            return csv;
        }
    }
}
