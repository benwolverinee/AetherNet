using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace AetherNet.Core
{
    public class AppFilter
    {
        private const string CONFIG_FILE = "filter_config.json";
        private const string FIREWALL_RULE_NAME = "AetherNet_AppFilter";

        public static FilterConfig LoadConfig()
        {
            try
            {
                if (File.Exists(CONFIG_FILE))
                {
                    var json = File.ReadAllText(CONFIG_FILE);
                    return JsonSerializer.Deserialize<FilterConfig>(json) ?? new FilterConfig();
                }
            }
            catch { }

            return new FilterConfig();
        }

        public static void ApplyFirewallRules()
        {
            try
            {
                // Önce eski kuralları temizle
                RemoveFirewallRules();

                var config = LoadConfig();
                
                if (config.Applications.Count == 0)
                {
                    // Filtre yok, tüm uygulamalar bypass kullanır
                    return;
                }

                if (config.Mode == "blacklist")
                {
                    // Blacklist: Seçilen uygulamaların WinDivert trafiğini engelle
                    foreach (var app in config.Applications)
                    {
                        BlockAppFromWinDivert(app);
                    }
                }
                // Whitelist modunda GoodbyeDPI zaten tüm trafiği işler, 
                // sadece seçili uygulamaların trafiğine izin vermek için
                // başka bir yaklaşım gerekir (process-based filtering)
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Firewall kuralları uygulanamadı: {ex.Message}");
            }
        }

        private static void BlockAppFromWinDivert(string appName)
        {
            try
            {
                // Windows Firewall kuralı oluştur
                var ruleName = $"{FIREWALL_RULE_NAME}_{appName}";
                
                var psi = new ProcessStartInfo
                {
                    FileName = "netsh",
                    Arguments = $"advfirewall firewall add rule name=\"{ruleName}\" dir=out program=\"{appName}\" action=block",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    Verb = "runas"
                };

                var process = Process.Start(psi);
                process?.WaitForExit();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Firewall kuralı eklenemedi ({appName}): {ex.Message}");
            }
        }

        public static void RemoveFirewallRules()
        {
            try
            {
                // Tüm AetherNet firewall kurallarını kaldır
                var psi = new ProcessStartInfo
                {
                    FileName = "netsh",
                    Arguments = $"advfirewall firewall delete rule name=all program=any",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true
                };

                var process = Process.Start(psi);
                var output = process?.StandardOutput.ReadToEnd();
                process?.WaitForExit();

                // Sadece AetherNet kurallarını sil
                if (output?.Contains(FIREWALL_RULE_NAME) == true)
                {
                    psi.Arguments = $"advfirewall firewall delete rule name=\"{FIREWALL_RULE_NAME}*\"";
                    process = Process.Start(psi);
                    process?.WaitForExit();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Firewall kuralları temizlenemedi: {ex.Message}");
            }
        }
    }

    public class FilterConfig
    {
        public string Mode { get; set; } = "whitelist";
        public List<string> Applications { get; set; } = new List<string>();
    }
}
