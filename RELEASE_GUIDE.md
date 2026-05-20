# Release Yayınlama Rehberi

## Sorun: GitHub 100 MB Dosya Limiti

AetherNet.exe 170 MB olduğu için normal git push çalışmaz. GitHub Releases kullanmamız gerekiyor (2 GB'a kadar destekler).

## Çözüm: İki Repository Sistemi

### 1. Private Repo (benwolverinee/AetherNet)
- Kaynak kodları içerir
- Sadece sen görebilirsin
- Kullanıcılar erişemez

### 2. Public Repo (benwolverinee/AetherNet-Releases)
- Sadece `version.json` içerir (küçük dosya)
- GitHub Releases'te exe dosyası yayınlanır
- Kullanıcılar buradan indirir

## Release Yapma (2 Yöntem)

### Yöntem 1: Otomatik (GitHub CLI ile) ⭐ ÖNERİLEN

#### GitHub CLI Kurulum (Tek Seferlik)

1. İndir: https://cli.github.com/
2. Kur
3. Terminal'de çalıştır:
```bash
gh auth login
```
4. GitHub hesabınla giriş yap

#### Release Oluştur

```bash
release-auto.bat
```

Bu script:
1. Version numarasını sorar
2. Build yapar
3. Private repo'ya commit/push yapar
4. Public repo'ya version.json yükler
5. GitHub Release oluşturur ve exe'yi yükler

✅ Tamamen otomatik!

### Yöntem 2: Manuel (GitHub CLI olmadan)

```bash
release-public.bat
```

Bu script:
1. Version numarasını sorar
2. Build yapar
3. Private repo'ya commit/push yapar
4. `D:\AetherNet-Releases\version.json` oluşturur
5. Sana manuel adımları gösterir

#### Manuel Adımlar:

**1. version.json'u public repo'ya yükle:**
```bash
cd D:\AetherNet-Releases
git add version.json
git commit -m "Update to v1.1"
git push
```

**2. GitHub Release oluştur:**

GitHub CLI ile:
```bash
gh release create v1.1 "releases\v1.1\AetherNet.exe" --repo benwolverinee/AetherNet-Releases --title "AetherNet v1.1" --notes "Discord ve engellenmiş siteler için DPI bypass"
```

VEYA tarayıcıda:
1. https://github.com/benwolverinee/AetherNet-Releases/releases/new
2. Tag: `v1.1`
3. Title: `AetherNet v1.1`
4. Description: "Discord ve engellenmiş siteler için DPI bypass"
5. `releases\v1.1\AetherNet.exe` dosyasını sürükle-bırak
6. **Publish release**

## Kullanıcılar Nasıl İndirir?

Kullanıcılar bu linkten indirir:
```
https://github.com/benwolverinee/AetherNet-Releases/releases/latest/download/AetherNet.exe
```

Veya releases sayfasından:
```
https://github.com/benwolverinee/AetherNet-Releases/releases
```

## Otomatik Güncelleme Nasıl Çalışır?

1. Uygulama başladığında `AutoUpdater.cs` çalışır
2. `https://raw.githubusercontent.com/benwolverinee/AetherNet-Releases/main/version.json` kontrol eder
3. Yeni version varsa kullanıcıya sorar
4. Kabul ederse `https://github.com/benwolverinee/AetherNet-Releases/releases/latest/download/AetherNet.exe` indirir
5. Uygulamayı kapatır ve günceller
6. Yeni versiyonu başlatır

## Örnek Release Süreci

```bash
# 1. Otomatik release yap
release-auto.bat

# Yeni version: 1.2
# Build yapılıyor...
# Private repo güncelleniyor...
# Public repo hazırlanıyor...
# GitHub Release oluşturuluyor...
# TAMAMLANDI!

# 2. Kontrol et
# https://github.com/benwolverinee/AetherNet-Releases/releases
```

## Sorun Giderme

### "gh: command not found"
GitHub CLI yüklü değil. `release-public.bat` kullan (manuel yöntem).

### "git push" çok yavaş
Normal, 170 MB dosya yüklüyorsun. GitHub Releases kullan.

### Kullanıcılar eski versiyonu görüyor
1. `version.json` güncel mi kontrol et
2. GitHub Release yayınlandı mı kontrol et
3. Kullanıcı uygulamayı yeniden başlatsın

### Public repo'da kod görünüyor
Yanlış repo'ya push yapmışsın. Public repo'da sadece `version.json` olmalı.

## Güvenlik

✅ Kaynak kodlar private repo'da gizli
✅ Kullanıcılar sadece exe indirebilir
✅ Kod çalınamaz
✅ Otomatik güncelleme çalışır
