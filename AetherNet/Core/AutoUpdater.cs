using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace AetherNet.Core
{
    public class AutoUpdater
    {
        private const string UPDATE_URL = "https://raw.githubusercontent.com/standme321/AetherNet/main/version.json";
        private const string DOWNLOAD_URL = "https://github.com/standme321/AetherNet/releases/latest/download/AetherNet.exe";
        private static readonly HttpClient _httpClient = new HttpClient();

        public static async Task<bool> CheckForUpdates()
        {
            try
            {
                string currentVersion = GetCurrentVersion();
                string latestVersion = await GetLatestVersion();

                if (string.IsNullOrEmpty(latestVersion))
                    return false;

                return IsNewerVersion(latestVersion, currentVersion);
            }
            catch
            {
                return false;
            }
        }

        public static async Task<bool> DownloadAndInstallUpdate()
        {
            try
            {
                string tempPath = Path.Combine(Path.GetTempPath(), "AetherNet_Update.exe");
                
                // Yeni versiyonu indir
                var response = await _httpClient.GetAsync(DOWNLOAD_URL);
                response.EnsureSuccessStatusCode();
                
                await using var fs = new FileStream(tempPath, FileMode.Create);
                await response.Content.CopyToAsync(fs);
                
                // Güncelleme script'i oluştur
                string batchPath = Path.Combine(Path.GetTempPath(), "update_aethernet.bat");
                string currentExe = Process.GetCurrentProcess().MainModule?.FileName ?? "";
                
                string batchContent = $@"@echo off
timeout /t 2 /nobreak > nul
taskkill /F /IM AetherNet.exe > nul 2>&1
timeout /t 1 /nobreak > nul
copy /Y ""{tempPath}"" ""{currentExe}""
del ""{tempPath}""
start """" ""{currentExe}""
del ""%~f0""
";
                
                File.WriteAllText(batchPath, batchContent);
                
                // Batch dosyasını çalıştır ve uygulamayı kapat
                Process.Start(new ProcessStartInfo
                {
                    FileName = batchPath,
                    CreateNoWindow = true,
                    UseShellExecute = false
                });
                
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static string GetCurrentVersion()
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var version = assembly.GetName().Version;
            return $"{version?.Major}.{version?.Minor}.{version?.Build}";
        }

        private static async Task<string> GetLatestVersion()
        {
            try
            {
                var response = await _httpClient.GetStringAsync(UPDATE_URL);
                var versionInfo = JsonSerializer.Deserialize<VersionInfo>(response);
                return versionInfo?.Version ?? "";
            }
            catch
            {
                return "";
            }
        }

        private static bool IsNewerVersion(string latest, string current)
        {
            try
            {
                var latestParts = latest.Split('.');
                var currentParts = current.Split('.');

                for (int i = 0; i < Math.Min(latestParts.Length, currentParts.Length); i++)
                {
                    if (int.Parse(latestParts[i]) > int.Parse(currentParts[i]))
                        return true;
                    if (int.Parse(latestParts[i]) < int.Parse(currentParts[i]))
                        return false;
                }

                return latestParts.Length > currentParts.Length;
            }
            catch
            {
                return false;
            }
        }

        private class VersionInfo
        {
            public string Version { get; set; } = "";
            public string DownloadUrl { get; set; } = "";
        }
    }
}
