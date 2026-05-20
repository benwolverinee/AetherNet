using System;
using System.Diagnostics;
using System.IO;

namespace AetherNet.Core
{
    public class WinDivertBypass : IDisposable
    {
        private Process? _goodbyeDpiProcess;

        public bool Start()
        {
            try
            {
                string basePath = AppDomain.CurrentDomain.BaseDirectory;
                string goodbyeDpiPath = Path.Combine(basePath, "Native", "goodbyedpi.exe");
                string blacklistPath = Path.Combine(basePath, "Native", "blacklist.txt");
                
                if (!File.Exists(goodbyeDpiPath))
                {
                    Debug.WriteLine($"goodbyedpi.exe bulunamadı: {goodbyeDpiPath}");
                    return false;
                }

                // GoodbyeDPI parametreleri - Blacklist modu (sadece Discord ve engellenmiş siteler)
                string arguments = $"-5 --set-ttl 5 --dns-addr 77.88.8.8 --dns-port 1253 --dnsv6-addr 2a02:6b8::feed:0ff --dnsv6-port 1253 --blacklist \"{blacklistPath}\"";
                
                var psi = new ProcessStartInfo
                {
                    FileName = goodbyeDpiPath,
                    Arguments = arguments,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    WorkingDirectory = Path.Combine(basePath, "Native")
                };

                _goodbyeDpiProcess = Process.Start(psi);
                
                if (_goodbyeDpiProcess == null)
                {
                    Debug.WriteLine("GoodbyeDPI process başlatılamadı");
                    return false;
                }

                Debug.WriteLine($"✅ GoodbyeDPI başlatıldı (PID: {_goodbyeDpiProcess.Id}, Blacklist Mode)");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GoodbyeDPI başlatma hatası: {ex.Message}");
                return false;
            }
        }

        public void Stop()
        {
            if (_goodbyeDpiProcess != null && !_goodbyeDpiProcess.HasExited)
            {
                try
                {
                    _goodbyeDpiProcess.Kill();
                    _goodbyeDpiProcess.WaitForExit(2000);
                    Debug.WriteLine("✅ GoodbyeDPI durduruldu");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"GoodbyeDPI durdurma hatası: {ex.Message}");
                }
                finally
                {
                    _goodbyeDpiProcess.Dispose();
                    _goodbyeDpiProcess = null;
                }
            }
        }

        public void Dispose()
        {
            Stop();
            GC.SuppressFinalize(this);
        }
    }
}
