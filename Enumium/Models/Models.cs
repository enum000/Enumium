namespace Enumium.Models
{
    public class TweakItem
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Impact { get; set; } = "Low"; // Low, Medium, High
        public bool IsEnabled { get; set; }
        public bool OriginalValue { get; set; }
        public string RegistryPath { get; set; } = string.Empty;
        public string RegistryValue { get; set; } = string.Empty;
        public object? EnabledData { get; set; }
        public object? DisabledData { get; set; }
        public string ServiceName { get; set; } = string.Empty;
    }

    public class CleanerItem
    {
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public long Size { get; set; }
        public int FileCount { get; set; }
        public bool IsSelected { get; set; } = true;
        public string Icon { get; set; } = "🗑️";
    }

    public class StartupItem
    {
        public string Name { get; set; } = string.Empty;
        public string Publisher { get; set; } = string.Empty;
        public string Command { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty; // Registry or Startup Folder
        public string RegistryPath { get; set; } = string.Empty;
        public string RegistryValueName { get; set; } = string.Empty;
        public bool IsEnabled { get; set; }
        public string Impact { get; set; } = "Unknown";
        public string FilePath { get; set; } = string.Empty;
    }

    public class NetworkAdapterInfo
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public string SubnetMask { get; set; } = string.Empty;
        public string Gateway { get; set; } = string.Empty;
        public string DnsServers { get; set; } = string.Empty;
        public string MacAddress { get; set; } = string.Empty;
        public long Speed { get; set; }
        public long BytesSent { get; set; }
        public long BytesReceived { get; set; }
    }

    public class PrivacyItem
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public bool IsEnabled { get; set; } // true = privacy protected (telemetry OFF)
        public string RegistryPath { get; set; } = string.Empty;
        public string RegistryValue { get; set; } = string.Empty;
        public object? ProtectedData { get; set; }
        public object? DefaultData { get; set; }
    }

    public class ProcessInfo
    {
        public int Pid { get; set; }
        public string Name { get; set; } = string.Empty;
        public double CpuUsage { get; set; }
        public long MemoryMB { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class OptimizationRecord
    {
        public DateTime Timestamp { get; set; }
        public string Action { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
        public bool Success { get; set; }
    }

    public class ServiceItem
    {
        public string Name { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string StartupType { get; set; } = string.Empty;
        public bool SafeToDisable { get; set; }
    }

    public class DnsPreset
    {
        public string Name { get; set; } = string.Empty;
        public string Primary { get; set; } = string.Empty;
        public string Secondary { get; set; } = string.Empty;
    }

    public class BenchmarkResult
    {
        public string TestName { get; set; } = string.Empty;
        public double Score { get; set; }
        public string Unit { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }
}
