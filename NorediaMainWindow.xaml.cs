using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Noredia.Services;

namespace Noredia
{
    public partial class MainWindow : Window
    {
        private readonly DownloadService _service = new();
        private string _selectedFormat = "Video";
        private Button[] _formatButtons;
        private CancellationTokenSource? _cts;

        public MainWindow()
        {
            InitializeComponent();
            _formatButtons = new[] { FmtVideo, FmtAudio, FmtImage };
            UpdateFormatButtons();

            // yt-dlp arka planda güncelle
            _ = Task.Run(async () => { try { await _service.UpdateYtDlpAsync(); } catch { } });
        }

        private void TitleBar_Drag(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left) DragMove();
        }

        private void BtnMin_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;
        private void BtnClose_Click(object sender, RoutedEventArgs e) => Application.Current.Shutdown();

        private void BtnPaste_Click(object sender, RoutedEventArgs e)
        {
            try { if (Clipboard.ContainsText()) TxtUrl.Text = Clipboard.GetText(); } catch { }
        }

        private void TxtUrl_TextChanged(object sender, TextChangedEventArgs e)
        {
            string url = TxtUrl.Text.Trim().ToLower();

            if (url.Contains("youtube.com") || url.Contains("youtu.be")) ShowPlatform("YouTube", "▶");
            else if (url.Contains("instagram.com")) ShowPlatform("Instagram", "IG");
            else if (url.Contains("tiktok.com")) ShowPlatform("TikTok", "TT");
            else if (url.Contains("twitter.com") || url.Contains("x.com")) ShowPlatform("X / Twitter", "X");
            else if (url.Contains("facebook.com") || url.Contains("fb.watch")) ShowPlatform("Facebook", "FB");
            else if (url.Contains("reddit.com")) ShowPlatform("Reddit", "R");
            else if (url.Contains("pinterest.com")) ShowPlatform("Pinterest", "P");
            else if (url.Contains("twitch.tv")) ShowPlatform("Twitch", "TW");
            else if (url.Length > 10 && (url.StartsWith("http") || url.StartsWith("www."))) ShowPlatform("Web Sitesi", "◈");
            else PlatformBadge.Visibility = Visibility.Collapsed;
        }

        private void ShowPlatform(string name, string icon)
        {
            PlatformIcon.Text = icon;
            PlatformName.Text = name;
            PlatformBadge.Visibility = Visibility.Visible;
        }

        private void Fmt_Click(object sender, RoutedEventArgs e)
        {
            _selectedFormat = ((Button)sender).Tag?.ToString() ?? "Video";
            UpdateFormatButtons();
        }

        private void UpdateFormatButtons()
        {
            foreach (var btn in _formatButtons)
            {
                bool sel = btn.Tag?.ToString() == _selectedFormat;
                btn.Background = sel ? FindResource("FireBrushSmall") as LinearGradientBrush : FindResource("BgCard") as SolidColorBrush;
                btn.Foreground = sel ? FindResource("BgDark") as SolidColorBrush : FindResource("TextDim") as SolidColorBrush;
                btn.FontWeight = sel ? FontWeights.Bold : FontWeights.Normal;
                btn.BorderBrush = sel ? FindResource("Accent") as SolidColorBrush : FindResource("BorderCol") as SolidColorBrush;
            }
        }

        private async void BtnDownload_Click(object sender, RoutedEventArgs e)
        {
            string url = TxtUrl.Text.Trim();

            if (string.IsNullOrWhiteSpace(url))
            {
                StatusText.Text = "Lütfen bir link girin";
                ProgressArea.Visibility = Visibility.Visible;
                return;
            }

            if (url.ToLower().Contains("instagram.com") && !_service.HasCookies())
            {
                var result = MessageBox.Show(
                    "Instagram indirmeleri için cookies.txt gereklidir.\n\nDevam ederseniz büyük ihtimalle HATA alacaksınız.\n\nYine de denemek ister misiniz?",
                    "Instagram — Giriş Gerekli",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result != MessageBoxResult.Yes) return;
            }

            if (!_service.IsYtDlpAvailable())
            {
                MessageBox.Show(
                    "yt-dlp bulunamadı!\n\nLütfen yt-dlp.exe ve ffmpeg.exe dosyalarını uygulamanın yanına kopyalayın.",
                    "Gerekli Program Eksik", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // UI Hazırlık
            BtnDownload.IsEnabled = false;
            ProgressArea.Visibility = Visibility.Visible;
            Progress.IsIndeterminate = true;
            Progress.Value = 0;
            StatusText.Text = "Hazırlanıyor...";

            string format = _selectedFormat == "Audio" ? "MP3" : (_selectedFormat == "Image" ? "Resim" : "Video");
            string quality = "En İyi (Önerilen)";
            if (CmbQuality.SelectedItem is ComboBoxItem cbi) quality = cbi.Content?.ToString() ?? "En İyi (Önerilen)";

            // Son İndirme Kartını Göster
            LastDownloadCard.Visibility = Visibility.Visible;
            LastUrl.Text = url.Length > 45 ? url.Substring(0, 45) + "..." : url;
            LastFormat.Text = format;
            LastStatus.Text = "İndiriliyor";
            LastProgressTxt.Text = "%0";

            _cts = new CancellationTokenSource();

            try
            {
                var progressHandler = new Progress<double>(p =>
                {
                    int pct = (int)(p * 100);
                    Progress.IsIndeterminate = false;
                    Progress.Value = pct;
                    StatusText.Text = $"İndiriliyor... %{pct}";
                    LastProgressTxt.Text = $"%{pct}";
                });

                var downloadResult = await _service.DownloadMediaAsync(
                    url, format, quality, _service.GetDownloadFolder(), progressHandler, _cts.Token);

                if (downloadResult.Success)
                {
                    LastStatus.Text = "Tamamlandı ✅";
                    LastProgressTxt.Text = "%100";
                    Progress.Value = 100;
                    StatusText.Text = "İndirme tamamlandı! ✅";

                    // 3 saniye sonra son indirme kartını gizle (Otomatik Temizleme)
                    _ = AutoClearHistoryAsync();
                }
                else
                {
                    LastStatus.Text = "Hata ❌";
                    StatusText.Text = "İndirme başarısız";
                    MessageBox.Show(downloadResult.Error, "İndirme Hatası", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (OperationCanceledException)
            {
                LastStatus.Text = "İptal Edildi";
                StatusText.Text = "İndirme iptal edildi";
            }
            catch (Exception ex)
            {
                LastStatus.Text = "Hata ❌";
                StatusText.Text = "İndirme başarısız";
                MessageBox.Show(ex.Message, "İndirme Hatası", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                BtnDownload.IsEnabled = true;
            }
        }

        private async Task AutoClearHistoryAsync()
        {
            await Task.Delay(3000); // 3 saniye bekle
            await Dispatcher.InvokeAsync(() =>
            {
                LastDownloadCard.Visibility = Visibility.Collapsed;
                ProgressArea.Visibility = Visibility.Collapsed;
                StatusText.Text = "";
            });
        }

        private void BtnOpenFolder_Click(object sender, RoutedEventArgs e)
        {
            string folder = _service.GetDownloadFolder();
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
            Process.Start("explorer.exe", folder);
        }

        private void BtnSettings_Click(object sender, RoutedEventArgs e)
        {
            string cookieStatus = _service.HasCookies()
                ? "✅ cookies.txt bulundu (Instagram desteği aktif)"
                : "❌ cookies.txt bulunamadı (Instagram indirmeleri çalışmaz)";

            SettingsCookieStatus.Text = cookieStatus;
            SettingsFolderPath.Text = _service.GetDownloadFolder();

            SettingsOverlay.Visibility = Visibility.Visible;
        }

        private void CloseSettings_Click(object sender, RoutedEventArgs e)
        {
            SettingsOverlay.Visibility = Visibility.Collapsed;
        }
    }
}