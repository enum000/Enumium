using Microsoft.Win32;

namespace Enumium.Services
{
    public class PrivacyService
    {
        public static List<Models.PrivacyItem> GetPrivacyItems()
        {
            return new List<Models.PrivacyItem>
            {
                // ── Telemetry & Data Collection ──
                new() { Id = "telemetry", Name = "Disable Telemetry", Category = "Telemetry",
                    Description = "Stops Windows from collecting and sending diagnostic data to Microsoft",
                    RegistryPath = @"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\DataCollection",
                    RegistryValue = "AllowTelemetry", ProtectedData = 0, DefaultData = 3 },

                new() { Id = "telemetry_service", Name = "Disable Connected User Experience", Category = "Telemetry",
                    Description = "Disables the DiagTrack service responsible for telemetry data transmission",
                    RegistryPath = @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\DiagTrack",
                    RegistryValue = "Start", ProtectedData = 4, DefaultData = 2 },

                new() { Id = "app_telemetry", Name = "Disable App Telemetry", Category = "Telemetry",
                    Description = "Prevents applications from sending usage data",
                    RegistryPath = @"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\AppCompat",
                    RegistryValue = "AITEnable", ProtectedData = 0, DefaultData = 1 },

                // ── Cortana & Search ──
                new() { Id = "cortana", Name = "Disable Cortana", Category = "Cortana & Search",
                    Description = "Disables Cortana virtual assistant and its data collection",
                    RegistryPath = @"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\Windows Search",
                    RegistryValue = "AllowCortana", ProtectedData = 0, DefaultData = 1 },

                new() { Id = "web_search", Name = "Disable Web Search", Category = "Cortana & Search",
                    Description = "Prevents search from querying the web and sending data to Microsoft",
                    RegistryPath = @"HKEY_CURRENT_USER\SOFTWARE\Policies\Microsoft\Windows\Explorer",
                    RegistryValue = "DisableSearchBoxSuggestions", ProtectedData = 1, DefaultData = 0 },

                new() { Id = "bing_search", Name = "Disable Bing Search", Category = "Cortana & Search",
                    Description = "Disables Bing web results in Windows Start menu search",
                    RegistryPath = @"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Search",
                    RegistryValue = "BingSearchEnabled", ProtectedData = 0, DefaultData = 1 },

                // ── Location ──
                new() { Id = "location", Name = "Disable Location Tracking", Category = "Location",
                    Description = "Prevents Windows and apps from accessing your location",
                    RegistryPath = @"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\LocationAndSensors",
                    RegistryValue = "DisableLocation", ProtectedData = 1, DefaultData = 0 },

                // ── Advertising ──
                new() { Id = "advertising_id", Name = "Disable Advertising ID", Category = "Advertising",
                    Description = "Prevents apps from using your advertising ID for targeted ads",
                    RegistryPath = @"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\AdvertisingInfo",
                    RegistryValue = "Enabled", ProtectedData = 0, DefaultData = 1 },

                new() { Id = "suggested_content", Name = "Disable Suggested Content", Category = "Advertising",
                    Description = "Removes suggested content and ads from Windows Settings",
                    RegistryPath = @"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\ContentDeliveryManager",
                    RegistryValue = "SubscribedContent-338393Enabled", ProtectedData = 0, DefaultData = 1 },

                new() { Id = "tips_tricks", Name = "Disable Tips & Tricks", Category = "Advertising",
                    Description = "Stops Windows from showing tips and suggestions",
                    RegistryPath = @"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\ContentDeliveryManager",
                    RegistryValue = "SoftLandingEnabled", ProtectedData = 0, DefaultData = 1 },

                // ── Diagnostic Data ──
                new() { Id = "diagnostic_data", Name = "Disable Diagnostic Data", Category = "Diagnostics",
                    Description = "Minimizes diagnostic data sent to Microsoft",
                    RegistryPath = @"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\DataCollection",
                    RegistryValue = "LimitDiagnosticLogCollection", ProtectedData = 1, DefaultData = 0 },

                new() { Id = "feedback", Name = "Disable Feedback Requests", Category = "Diagnostics",
                    Description = "Stops Windows from asking for feedback",
                    RegistryPath = @"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Siuf\Rules",
                    RegistryValue = "NumberOfSIUFInPeriod", ProtectedData = 0, DefaultData = 1 },

                new() { Id = "error_reporting", Name = "Disable Error Reporting", Category = "Diagnostics",
                    Description = "Prevents Windows from sending error reports to Microsoft",
                    RegistryPath = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\Windows Error Reporting",
                    RegistryValue = "Disabled", ProtectedData = 1, DefaultData = 0 },

                // ── Activity History ──
                new() { Id = "activity_history", Name = "Disable Activity History", Category = "Activity",
                    Description = "Stops Windows from collecting your activity history",
                    RegistryPath = @"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\System",
                    RegistryValue = "EnableActivityFeed", ProtectedData = 0, DefaultData = 1 },

                new() { Id = "activity_upload", Name = "Disable Activity Upload", Category = "Activity",
                    Description = "Prevents uploading activity history to Microsoft cloud",
                    RegistryPath = @"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\System",
                    RegistryValue = "UploadUserActivities", ProtectedData = 0, DefaultData = 1 },

                new() { Id = "timeline", Name = "Disable Timeline", Category = "Activity",
                    Description = "Disables Windows Timeline feature that tracks your activities",
                    RegistryPath = @"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\System",
                    RegistryValue = "PublishUserActivities", ProtectedData = 0, DefaultData = 1 },

                // ── WiFi ──
                new() { Id = "wifi_sense", Name = "Disable WiFi Sense", Category = "Network Privacy",
                    Description = "Prevents automatic sharing of WiFi passwords with contacts",
                    RegistryPath = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\WcmSvc\wifinetworkmanager\config",
                    RegistryValue = "AutoConnectAllowedOEM", ProtectedData = 0, DefaultData = 1 },
            };
        }

        public static bool ReadPrivacyState(Models.PrivacyItem item)
        {
            try
            {
                var val = Registry.GetValue(item.RegistryPath, item.RegistryValue, null);
                if (val != null && item.ProtectedData != null)
                    return val.ToString() == item.ProtectedData.ToString();
            }
            catch { }
            return false;
        }

        public static bool ApplyPrivacy(Models.PrivacyItem item, bool protect)
        {
            try
            {
                var data = protect ? item.ProtectedData : item.DefaultData;
                if (data is int intVal)
                    Registry.SetValue(item.RegistryPath, item.RegistryValue, intVal, RegistryValueKind.DWord);
                else
                    Registry.SetValue(item.RegistryPath, item.RegistryValue, data);
                return true;
            }
            catch { return false; }
        }

        public static double CalculatePrivacyScore(List<Models.PrivacyItem> items)
        {
            if (items.Count == 0) return 0;
            int enabled = items.Count(i => i.IsEnabled);
            return Math.Round((double)enabled / items.Count * 100, 0);
        }
    }
}
