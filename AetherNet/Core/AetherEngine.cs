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
                // ServiceController'ı yeniden oluştur (cache sorununu önlemek için)
                ServiceController? service = null;
                
                try
                {
                    service = new ServiceController(SERVICE_NAME);
                    service.Refresh();
                    
                    // Servisin durumunu kontrol et
                    var currentStatus = service.Status;
                    
                    if (currentStatus == ServiceControllerStatus.Stopped)
                    {
                        service.Start();
                        service.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(10));
                    }
                    else if (currentStatus == ServiceControllerStatus.Paused)
                    {
                        service.Continue();
                        service.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(10));
                    }
                    else if (currentStatus == ServiceControllerStatus.Running)
                    {
                        // Zaten çalışıyor
                    }

                    _isRunning = true;

                    // DPI bypass'ı başlat (GUI uygulamasından)
                    _winDivertBypass = new WinDivertBypass();
                    _winDivertBypass.Start();
                }
                finally
                {
                    service?.Dispose();
                }
            }
            catch (InvalidOperationException ex)
            {
                throw new Exception($"Servis bulunamadı. Lütfen uygulamayı yeniden başlatın. Detay: {ex.Message}");
            }
            catch (System.ComponentModel.Win32Exception ex)
            {
                throw new Exception($"Yetki hatası. Uygulamayı yönetici olarak çalıştırın. Detay: {ex.Message}");
            }
            catch (System.TimeoutException)
            {
                throw new Exception("Servis başlatılamadı (zaman aşımı). Lütfen tekrar deneyin.");
            }
            catch (Exception ex)
            {
                throw new Exception($"Başlatma hatası: {ex.Message}");
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
                    service.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(3));
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
                service.Refresh();
                var status = service.Status; // Status'u okuyarak servisin gerçekten var olduğunu kontrol et
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool InstallService()
        {
            try
            {
                string servicePath = GetServicePath();
                
                if (!File.Exists(servicePath))
                {
                    return false;
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
                        UninstallService();
                        System.Threading.Thread.Sleep(300);
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
                    return false;
                }

                System.Threading.Thread.Sleep(300);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool UninstallService()
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
                        service.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(3));
                    }
                }
                catch { }

                System.Threading.Thread.Sleep(300);

                var psi = new ProcessStartInfo
                {
                    FileName = "sc.exe",
                    Arguments = $"delete \"{SERVICE_NAME}\"",
                    UseShellExecute = true,
                    CreateNoWindow = true,
                    Verb = "runas"
                };

                var process = Process.Start(psi);
                process?.WaitForExit();

                if (process?.ExitCode != 0)
                {
                    return false;
                }

                return true;
            }
            catch
            {
                return false;
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
