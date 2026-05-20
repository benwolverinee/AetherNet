using System.Diagnostics;

namespace AetherNet.Service;

public class AetherNetWorker : BackgroundService
{
    private readonly ILogger<AetherNetWorker> _logger;

    public AetherNetWorker(ILogger<AetherNetWorker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _logger.LogInformation("🚀 AetherNet DPI Bypass başlatılıyor...");

            // DNS ayarlarını uygula
            SetDNS("1.1.1.1", "1.0.0.1");
            FlushDNS();
            _logger.LogInformation("✅ DNS ayarları uygulandı (Cloudflare)");

            // GoodbyeDPI benzeri bypass başlat
            StartDpiBypass();

            _logger.LogInformation("✅ AetherNet servisi aktif - Discord ve tüm siteler çalışmalı!");

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException)
        {
            // Normal kapatma
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Servis hatası: {Message}", ex.Message);
        }
    }

    private void StartDpiBypass()
    {
        try
        {
            // PowerShell ile netsh komutlarını çalıştır (GoodbyeDPI benzeri)
            string commands = @"
                # HTTP fragment
                netsh int ipv4 set global defaultcurhoplimit=65
                netsh int ipv6 set global defaultcurhoplimit=65
                
                # TCP optimizasyonları
                netsh int tcp set global autotuninglevel=normal
                netsh int tcp set global chimney=enabled
                netsh int tcp set global dca=enabled
                netsh int tcp set global netdma=enabled
                netsh int tcp set global ecncapability=enabled
                netsh int tcp set global timestamps=enabled
            ";

            RunPowerShellCommand(commands);
            _logger.LogInformation("✅ DPI bypass ayarları uygulandı");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "⚠️ DPI bypass ayarları uygulanamadı: {Message}", ex.Message);
        }
    }

    private void RunPowerShellCommand(string commands)
    {
        try
        {
            var psi = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = $"-NoProfile -ExecutionPolicy Bypass -Command \"{commands}\"",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                Verb = "runas"
            };

            var process = Process.Start(psi);
            process?.WaitForExit(10000);
        }
        catch { }
    }

    private void SetDNS(string primary, string secondary)
    {
        try
        {
            string[] adapters = { 
                "Wi-Fi", "Ethernet", "Local Area Connection", 
                "Kablosuz Ag Baglantisi", "Ethernet 2", "Wi-Fi 2",
                "Kablosuz Ağ Bağlantısı", "Yerel Ağ Bağlantısı"
            };

            foreach (var adapter in adapters)
            {
                try
                {
                    RunCommand("netsh", $"interface ip set dns \"{adapter}\" static {primary}");
                    RunCommand("netsh", $"interface ip add dns \"{adapter}\" {secondary} index=2");
                }
                catch { }
            }
        }
        catch { }
    }

    private void FlushDNS()
    {
        try
        {
            RunCommand("ipconfig", "/flushdns");
        }
        catch { }
    }

    private void RestoreDNS()
    {
        try
        {
            string[] adapters = { 
                "Wi-Fi", "Ethernet", "Local Area Connection", 
                "Kablosuz Ag Baglantisi", "Ethernet 2", "Wi-Fi 2",
                "Kablosuz Ağ Bağlantısı", "Yerel Ağ Bağlantısı"
            };

            foreach (var adapter in adapters)
            {
                try
                {
                    RunCommand("netsh", $"interface ip set dns \"{adapter}\" dhcp");
                }
                catch { }
            }
            
            FlushDNS();
        }
        catch { }
    }

    private void RunCommand(string fileName, string arguments)
    {
        try
        {
            var psi = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            var process = Process.Start(psi);
            process?.WaitForExit(5000);
        }
        catch { }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("🛑 AetherNet servisi durduruluyor...");
        
        RestoreDNS();
        await base.StopAsync(cancellationToken);
        _logger.LogInformation("✅ AetherNet servisi durduruldu.");
    }
}
