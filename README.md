# AetherNet - DPI Bypass Tool

Türkiye'de Discord, Telegram ve engellenmiş sitelere erişim için DPI bypass aracı.

## Özellikler

✅ Discord uygulaması desteği  
✅ Telegram desteği  
✅ Tüm engellenmiş web siteleri  
✅ Windows servisi olarak çalışır  
✅ Otomatik güncelleme  
✅ Tek .exe dosyası  

## Kurulum

1. **AetherNet.exe**'yi indir
2. **Yönetici olarak çalıştır**
3. **"Servis Yükle"** butonuna tıkla
4. **"BAŞLAT"** butonuna tıkla
5. Discord'u aç!

## Kullanım

- **Servis Yükle**: Windows servisini yükler (bir kez yapılır)
- **BAŞLAT**: DPI bypass'ı başlatır
- **DURDUR**: DPI bypass'ı durdurur
- **Servis Kaldır**: Windows servisini kaldırır

## Güncelleme

Uygulama otomatik olarak güncellemeleri kontrol eder. Yeni versiyon varsa bildirim alırsınız.

## Teknik Detaylar

- **GoodbyeDPI Turkey**: Türkiye için özel optimize edilmiş
- **DNS Bypass**: Cloudflare DNS (1.1.1.1) + Yandex DNS (77.88.8.8)
- **Paket Manipülasyonu**: SNI fragmentation, TTL değiştirme
- **Windows Servisi**: Arka planda çalışır, PC yeniden başlatılsa bile aktif

## Geliştirici Notları

### Tek EXE Oluşturma

```bash
publish.bat
```

### Güncelleme Yayınlama

1. `AetherNet.csproj`'deki `<Version>` değerini artır
2. `version.json`'daki version'ı güncelle
3. `publish.bat` çalıştır
4. GitHub'da yeni release oluştur
5. `publish\AetherNet.exe`'yi release'e yükle

### GitHub Kurulumu

1. GitHub repository oluştur
2. `version.json`'ı repository'ye yükle (main branch)
3. `AutoUpdater.cs`'deki `YOUR_USERNAME`'i değiştir
4. Release oluştur ve exe'yi yükle

## Lisans

MIT License

## Uyarı

Bu araç sadece eğitim amaçlıdır. Kullanımdan doğacak sorumluluk kullanıcıya aittir.
