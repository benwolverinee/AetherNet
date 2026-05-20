using System;
using System.Windows;
using System.Windows.Media;
using AetherNet.Core;

namespace AetherNet.Views
{
    public partial class MainWindow : Window
    {
        private AetherEngine _engine = new AetherEngine();
        private bool _isRunning = false;

        private readonly SolidColorBrush ColorRed = new SolidColorBrush(Color.FromRgb(255, 76, 76));
        private readonly SolidColorBrush ColorGreen = new SolidColorBrush(Color.FromRgb(105, 240, 174));
        private readonly SolidColorBrush ColorCyan = new SolidColorBrush(Color.FromRgb(0, 229, 255));

        public MainWindow()
        {
            InitializeComponent();
            UpdateServiceButtonStates();
            CheckForUpdates();
        }

        private async void CheckForUpdates()
        {
            try
            {
                bool updateAvailable = await AutoUpdater.CheckForUpdates();
                
                if (updateAvailable)
                {
                    var result = MessageBox.Show(
                        "🔄 Yeni güncelleme mevcut!\n\nŞimdi güncellemek ister misiniz?",
                        "Güncelleme",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Information);

                    if (result == MessageBoxResult.Yes)
                    {
                        bool success = await AutoUpdater.DownloadAndInstallUpdate();
                        
                        if (success)
                        {
                            Application.Current.Shutdown();
                        }
                        else
                        {
                            MessageBox.Show(
                                "Güncelleme başarısız oldu.",
                                "Hata",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                        }
                    }
                }
            }
            catch { }
        }

        private void UpdateServiceButtonStates()
        {
            bool isInstalled = _engine.IsServiceInstalled();
            InstallServiceBtn.IsEnabled = !isInstalled;
            UninstallServiceBtn.IsEnabled = isInstalled;

            if (isInstalled)
            {
                InstallServiceBtn.Opacity = 0.5;
                UninstallServiceBtn.Opacity = 1.0;
            }
            else
            {
                InstallServiceBtn.Opacity = 1.0;
                UninstallServiceBtn.Opacity = 0.5;
            }
        }

        private void ToggleBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!_isRunning)
            {
                try
                {
                    _engine.Start();
                    _isRunning = true;

                    ToggleBtn.Content = "DURDUR";
                    ToggleBtn.BorderBrush = ColorRed;
                    StatusText.Text = "Sistem Aktif - DPI Bypass Calisiyor";
                    StatusText.Foreground = ColorGreen;
                }
                catch (Exception ex)
                {
                    UpdateServiceButtonStates(); // Buton durumlarını güncelle
                    MessageBox.Show(ex.Message, "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                _engine.Stop();
                _isRunning = false;

                ToggleBtn.Content = "BASLAT";
                ToggleBtn.BorderBrush = ColorCyan;
                StatusText.Text = "Sistem Kapali";
                StatusText.Foreground = ColorRed;
            }
        }

        private void InstallServiceBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var result = MessageBox.Show(
                    "AetherNet servisi yüklenecek.\n\n" +
                    "✅ Manuel başlatma (BAŞLAT butonu ile)\n" +
                    "✅ PC yeniden başlatılsa bile yüklü kalır\n\n" +
                    "Devam?",
                    "Servis Yükleme",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    _engine.InstallService();
                    UpdateServiceButtonStates();
                    
                    MessageBox.Show(
                        "✅ Servis yüklendi!\n\nŞimdi 'BAŞLAT' butonuna basın.",
                        "Başarılı",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Hata:\n\n" + ex.Message,
                    "Hata",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void UninstallServiceBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var result = MessageBox.Show(
                    "Servis kaldırılacak.\n\nDevam?",
                    "Servis Kaldırma",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    if (_isRunning)
                    {
                        _engine.Stop();
                        _isRunning = false;
                        ToggleBtn.Content = "BASLAT";
                        ToggleBtn.BorderBrush = ColorCyan;
                        StatusText.Text = "Sistem Kapali";
                        StatusText.Foreground = ColorRed;
                    }

                    _engine.UninstallService();
                    UpdateServiceButtonStates();
                    
                    MessageBox.Show(
                        "✅ Servis kaldırıldı!",
                        "Başarılı",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Hata:\n\n" + ex.Message,
                    "Hata",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_isRunning) _engine.Stop();
            Application.Current.Shutdown();
        }

        private void FilterBtn_Click(object sender, RoutedEventArgs e)
        {
            var filterWindow = new FilterWindow();
            filterWindow.ShowDialog();
        }
    }
}
