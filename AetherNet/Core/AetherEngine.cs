using System;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;

namespace AetherNet.Core
{
    public class AetherEngine : IDisposable
    {
        private const string SERVICE_NAME = "AetherNet DPI Bypass Service";
        private bool _isRunning;
        private WinDivertBypass? _winDivertBypass;

        public bool IsRunning => _isRunning;

        public void Start()
        {
            if (_isRunning) return;

            try
            {
                using var service = new ServiceController(SERVICE_NAME);
                
                if (service.Status == ServiceControllerStatus.Stopped)
                {
                    service.Start();
                    service.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(10));
                }
                else if (service.Status == ServiceControllerStatus.Paused)
                {
                    service.Continue();
                    service.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(10));
                }

                _isRunning = true;

                // DPI bypass'ı başlat (GUI uygulamasından)
                _winDivertBypass = new WinDivertBypass();
                _winDivertBypass.Start();
            }
            catch (InvalidOperationException)
            {
                throw new Exception($"'{SERVICE_NAME}' servisi bulunamadı!\n\nÖnce 'Servis Yükle' butonuna tıklayın.");
            }
            catch (Exception ex)
            {
                throw new Exception($"Servis başlatma hatası: {ex.Message}");
            }
        }

        public void Stop()
        {
            if (!_isRunning) return;

            try
            {
                // WinDivert bypass'ı durdur
                _winDivertBypass?.Stop();
                _winDivertBypass?.Dispose();
                _winDivertBypass = null;

                using var service = new ServiceController(SERVICE_NAME);
                
                if (service.Status == ServiceControllerStatus.Running)
                {
                    service.Stop();
                    service.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(10));
                }

                _isRunning = false;
            }
            catch { }
        }

        public bool IsServiceInstalled()
        {
            try
            {
                using var service = new ServiceController(SERVICE_NAME);
                var status = service.Status;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void InstallService()
        {
            try
            {
                string servicePath = GetServicePath();
                
                if (!File.Exists(servicePath))
                {
                    throw new Exception($"Servis dosyası bulunamadı!\n\nAranan: {servicePath}\n\nLütfen uygulamayı doğru klasörden çalıştırın.");
                }

                // Önce varsa eski servisi sil
                try
                {
                    var checkPsi = new ProcessStartInfo
                    {
                        FileName = "sc.exe",
                        Arguments = $"query \"{SERVICE_NAME}\"",
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        RedirectStandardOutput = true
                    };
                    var checkProcess = Process.Start(checkPsi);
                    checkProcess?.WaitForExit();
                    
                    if (checkProcess?.ExitCode == 0)
                    {
                        // Servis var, sil
                        UninstallService();
                        System.Threading.Thread.Sleep(1000);
                    }
                }
                catch { }

                var psi = new ProcessStartInfo
                {
                    FileName = "sc.exe",
                    Arguments = $"create \"{SERVICE_NAME}\" binPath=\"{servicePath}\" start=demand DisplayName=\"AetherNet DPI Bypass\"",
                    UseShellExecute = true,
                    CreateNoWindow = true,
                    Verb = "runas"
                };

                var process = Process.Start(psi);
                process?.WaitForExit();

                if (process?.ExitCode != 0)
                {
                    throw new Exception($"Servis yüklenemedi! Hata kodu: {process?.ExitCode}");
                }

                System.Threading.Thread.Sleep(1000);
            }
            catch (Exception ex)
            {
                throw new Exception($"Servis yükleme hatası: {ex.Message}");
            }
        }

        public void UninstallService()
        {
            try
            {
                // Önce servisi durdur
                try
                {
                    using var service = new ServiceController(SERVICE_NAME);
                    if (service.Status == ServiceControllerStatus.Running)
                    {
                        service.Stop();
                        service.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(10));
                    }
                }
                catch { }

                System.Threading.Thread.Sleep(1000); // Servisin tamamen durmasını bekle

                var psi = new ProcessStartInfo
                {
                    FileName = "sc.exe",
                    Arguments = $"delete \"{SERVICE_NAME}\"",
                    UseShellExecute = true,
                    CreateNoWindow = false, // Hata mesajlarını görmek için
                    Verb = "runas"
                };

                var process = Process.Start(psi);
                process?.WaitForExit();

                if (process?.ExitCode != 0)
                {
                    throw new Exception($"Servis kaldırılamadı! Exit code: {process?.ExitCode}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Servis kaldırma hatası: {ex.Message}");
            }
        }

        private string GetServicePath()
        {
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            return Path.Combine(basePath, "AetherNet.Service.exe");
        }

        public void Dispose()
        {
            Stop();
            _winDivertBypass?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
