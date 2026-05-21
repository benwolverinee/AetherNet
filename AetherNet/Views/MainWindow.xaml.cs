using System;
using System.Windows;
using System.Windows.Media;
using AetherNet.Core;
using System.Drawing;
using System.Windows.Forms;
using Application = System.Windows.Application;

namespace AetherNet.Views
{
    public partial class MainWindow : Window
    {
        private AetherEngine _engine = new AetherEngine();
        private bool _isRunning = false;
        private NotifyIcon? _notifyIcon;

        private readonly SolidColorBrush ColorRed = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 76, 76));
        private readonly SolidColorBrush ColorGreen = new SolidColorBrush(System.Windows.Media.Color.FromRgb(105, 240, 174));
        private readonly SolidColorBrush ColorOrange = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 107, 0));
        private readonly SolidColorBrush ColorYellow = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 184, 0));

        public MainWindow()
        {
            InitializeComponent();
            InitializeSystemTray();
            UpdateServiceButtonStates();
        }

        private void InitializeSystemTray()
        {
            _notifyIcon = new NotifyIcon
            {
                Icon = SystemIcons.Application,
                Visible = true,
                Text = "AetherNet"
            };

            _notifyIcon.DoubleClick += (s, e) =>
            {
                Show();
                WindowState = WindowState.Normal;
                Activate();
            };

            var contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add("Aç", null, (s, e) =>
            {
                Show();
                WindowState = WindowState.Normal;
                Activate();
            });
            contextMenu.Items.Add("-");
            contextMenu.Items.Add("Çıkış", null, (s, e) =>
            {
                _notifyIcon.Visible = false;
                if (_isRunning) _engine.Stop();
                Application.Current.Shutdown();
            });

            _notifyIcon.ContextMenuStrip = contextMenu;
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
                ServiceStatusText.Text = "Yüklü";
                ServiceStatusText.Foreground = ColorGreen;
            }
            else
            {
                InstallServiceBtn.Opacity = 1.0;
                UninstallServiceBtn.Opacity = 0.5;
                ServiceStatusText.Text = "Yüklü Değil";
                ServiceStatusText.Foreground = ColorRed;
            }
        }

        private void ToggleBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!_isRunning)
            {
                // Servis yüklü mü kontrol et
                if (!_engine.IsServiceInstalled())
                {
                    StatusText.Text = "Önce 'Servis Yükle'";
                    StatusText.Foreground = ColorRed;
                    
                    // 2 saniye sonra yazıyı kaldır
                    _ = Task.Run(async () =>
                    {
                        await Task.Delay(2000);
                        Dispatcher.Invoke(() => StatusText.Text = "");
                    });
                    return;
                }

                try
                {
                    _engine.Start();
                    _isRunning = true;

                    ToggleBtn.Content = "DURDUR";
                    StatusText.Text = "Sistem Aktif";
                    StatusText.Foreground = ColorGreen;
                    
                    // Durum ışığını yeşil yap (animasyonlu)
                    UpdateStatusIndicator(true);
                }
                catch (Exception ex)
                {
                    StatusText.Text = ex.Message;
                    StatusText.Foreground = ColorRed;
                    UpdateServiceButtonStates();
                }
            }
            else
            {
                _engine.Stop();
                _isRunning = false;

                ToggleBtn.Content = "BAŞLAT";
                StatusText.Text = "";
                StatusText.Foreground = ColorOrange;
                
                // Durum ışığını kırmızı yap (animasyonlu)
                UpdateStatusIndicator(false);
            }
        }

        private void UpdateStatusIndicator(bool isActive)
        {
            var colorAnimation = new System.Windows.Media.Animation.ColorAnimation
            {
                To = isActive ? System.Windows.Media.Color.FromRgb(105, 240, 174) : System.Windows.Media.Color.FromRgb(255, 76, 76),
                Duration = TimeSpan.FromMilliseconds(400),
                EasingFunction = new System.Windows.Media.Animation.CubicEase { EasingMode = System.Windows.Media.Animation.EasingMode.EaseOut }
            };

            var brush = new SolidColorBrush();
            brush.BeginAnimation(SolidColorBrush.ColorProperty, colorAnimation);
            StatusIndicator.Fill = brush;

            // Glow efekti
            var glowColor = isActive ? System.Windows.Media.Color.FromRgb(105, 240, 174) : System.Windows.Media.Color.FromRgb(255, 76, 76);
            StatusIndicator.Effect = new System.Windows.Media.Effects.DropShadowEffect
            {
                Color = glowColor,
                BlurRadius = 8,
                ShadowDepth = 0,
                Opacity = 0.8
            };
        }

        private async void InstallServiceBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                StatusText.Text = "Servis yükleniyor...";
                StatusText.Foreground = ColorYellow;
                InstallServiceBtn.IsEnabled = false;

                bool success = await Task.Run(() => _engine.InstallService());
                
                if (success)
                {
                    UpdateServiceButtonStates();
                    StatusText.Text = "Kapalı";
                    StatusText.Foreground = ColorOrange;
                }
                else
                {
                    StatusText.Text = "Servis yüklenemedi";
                    StatusText.Foreground = ColorRed;
                    InstallServiceBtn.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                StatusText.Text = "Hata: " + ex.Message;
                StatusText.Foreground = ColorRed;
                InstallServiceBtn.IsEnabled = true;
            }
        }

        private async void UninstallServiceBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_isRunning)
                {
                    _engine.Stop();
                    _isRunning = false;
                    ToggleBtn.Content = "BAŞLAT";
                }

                StatusText.Text = "Servis kaldırılıyor...";
                StatusText.Foreground = ColorYellow;
                UninstallServiceBtn.IsEnabled = false;

                bool success = await Task.Run(() => _engine.UninstallService());
                
                if (success)
                {
                    UpdateServiceButtonStates();
                    StatusText.Text = "";
                }
                else
                {
                    StatusText.Text = "Servis kaldırılamadı";
                    StatusText.Foreground = ColorRed;
                    UninstallServiceBtn.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                StatusText.Text = "Hata: " + ex.Message;
                StatusText.Foreground = ColorRed;
                UninstallServiceBtn.IsEnabled = true;
            }
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        protected override void OnClosed(EventArgs e)
        {
            _notifyIcon?.Dispose();
            _notifyIcon = null;
            
            if (_isRunning) 
            {
                _engine.Stop();
            }
            
            _engine?.Dispose();
            _engine = null;
            
            base.OnClosed(e);
            
            // Garbage collection'ı zorla
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        private void TitleBar_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ButtonState == System.Windows.Input.MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
    }
}
