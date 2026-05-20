using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

namespace AetherNet.Views
{
    public partial class FilterWindow : Window
    {
        private const string CONFIG_FILE = "filter_config.json";
        private FilterConfig _config = new FilterConfig();

        public FilterWindow()
        {
            InitializeComponent();
            LoadConfig();
            LoadDefaultApps();
            UpdateModeDescription();
        }

        private void LoadConfig()
        {
            try
            {
                if (File.Exists(CONFIG_FILE))
                {
                    var json = File.ReadAllText(CONFIG_FILE);
                    _config = JsonSerializer.Deserialize<FilterConfig>(json) ?? new FilterConfig();
                }
                else
                {
                    _config = new FilterConfig();
                }
            }
            catch
            {
                _config = new FilterConfig();
            }

            WhitelistMode.IsChecked = _config.Mode == "whitelist";
            BlacklistMode.IsChecked = _config.Mode == "blacklist";
        }

        private void LoadDefaultApps()
        {
            // Varsayılan uygulamalar
            var defaultApps = new List<string>
            {
                "Discord.exe",
                "chrome.exe",
                "firefox.exe",
                "msedge.exe",
                "opera.exe",
                "brave.exe",
                "Telegram.exe"
            };

            // Mevcut uygulamaları ekle
            foreach (var app in _config.Applications)
            {
                if (!defaultApps.Contains(app, StringComparer.OrdinalIgnoreCase))
                {
                    defaultApps.Add(app);
                }
            }

            // UI'a ekle
            AppListPanel.Children.Clear();
            foreach (var app in defaultApps.OrderBy(a => a))
            {
                AddAppToList(app, _config.Applications.Contains(app, StringComparer.OrdinalIgnoreCase));
            }
        }

        private void AddAppToList(string appName, bool isChecked)
        {
            var panel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0, 5, 0, 5)
            };

            var checkbox = new CheckBox
            {
                Content = appName,
                IsChecked = isChecked,
                Foreground = System.Windows.Media.Brushes.White,
                FontSize = 12,
                Tag = appName,
                Width = 350
            };

            var deleteBtn = new Button
            {
                Content = "🗑️",
                Width = 30,
                Height = 25,
                Background = System.Windows.Media.Brushes.Transparent,
                Foreground = System.Windows.Media.Brushes.Gray,
                BorderThickness = new Thickness(0),
                Cursor = System.Windows.Input.Cursors.Hand,
                Tag = appName,
                Margin = new Thickness(10, 0, 0, 0)
            };
            deleteBtn.Click += DeleteApp_Click;

            panel.Children.Add(checkbox);
            panel.Children.Add(deleteBtn);

            AppListPanel.Children.Add(panel);
        }

        private void DeleteApp_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var appName = btn?.Tag as string;

            if (string.IsNullOrEmpty(appName))
                return;

            // Varsayılan uygulamaları silme
            var defaultApps = new[] { "Discord.exe", "chrome.exe", "firefox.exe", "msedge.exe", "Telegram.exe" };
            if (defaultApps.Contains(appName, StringComparer.OrdinalIgnoreCase))
            {
                MessageBox.Show("Varsayılan uygulamalar silinemez!", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // UI'dan kaldır
            if (btn?.Parent is StackPanel panel)
            {
                AppListPanel.Children.Remove(panel);
            }
        }

        private void AddAppBtn_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Executable Files (*.exe)|*.exe",
                Title = "Uygulama Seç"
            };

            if (dialog.ShowDialog() == true)
            {
                var fileName = Path.GetFileName(dialog.FileName);

                // Zaten var mı kontrol et
                foreach (StackPanel panel in AppListPanel.Children)
                {
                    var checkbox = panel.Children[0] as CheckBox;
                    if (checkbox?.Content.ToString().Equals(fileName, StringComparison.OrdinalIgnoreCase) == true)
                    {
                        MessageBox.Show("Bu uygulama zaten listede!", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }

                AddAppToList(fileName, true);
            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _config.Mode = WhitelistMode.IsChecked == true ? "whitelist" : "blacklist";
                _config.Applications.Clear();

                foreach (StackPanel panel in AppListPanel.Children)
                {
                    if (panel.Children[0] is CheckBox checkbox && checkbox.IsChecked == true)
                    {
                        var appName = checkbox.Content.ToString();
                        if (!string.IsNullOrEmpty(appName))
                        {
                            _config.Applications.Add(appName);
                        }
                    }
                }

                var json = JsonSerializer.Serialize(_config, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(CONFIG_FILE, json);

                MessageBox.Show("Ayarlar kaydedildi!", "Başarılı", MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Kaydetme hatası: {ex.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void FilterMode_Changed(object sender, RoutedEventArgs e)
        {
            UpdateModeDescription();
        }

        private void UpdateModeDescription()
        {
            if (ModeDescription == null) return;

            if (WhitelistMode?.IsChecked == true)
            {
                ModeDescription.Text = "Whitelist: Sadece seçilen uygulamalar bypass kullanır (Discord, Chrome vs.)";
            }
            else
            {
                ModeDescription.Text = "Blacklist: Seçilen uygulamalar HARİÇ tüm uygulamalar bypass kullanır (COD, Valorant hariç)";
            }
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }

    public class FilterConfig
    {
        public string Mode { get; set; } = "whitelist";
        public List<string> Applications { get; set; } = new List<string>();
    }
}
