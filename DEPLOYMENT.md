# AetherNet Otomatik Güncelleme Sistemi

## Nasıl Çalışır?

1. **Release script çalıştır** → `release-auto.bat`
2. **Otomatik build** yapar ve GitHub'a yükler
3. **Kullanıcılar** uygulamayı açınca otomatik güncelleme alır

## İki Repository Sistemi

### Private Repo (benwolverinee/AetherNet)
- Kaynak kodları içerir
- Sadece sen görebilirsin
- Kullanıcılar erişemez

### Public Repo (benwolverinee/AetherNet-Releases)
- Sadece `version.json` içerir
- GitHub Releases'te exe dosyası yayınlanır
- Kullanıcılar buradan indirir

## İlk Kurulum

### 1. Private Repository (Zaten Var)

```bash
# benwolverinee/AetherNet - PRIVATE
# Kaynak kodlar burada
```

### 2. Public Repository Oluştur

1. https://github.com/new
2. Repository name: `AetherNet-Releases`
3. **Public** seç
4. Create repository

### 3. Public Repo'yu Klonla

```bash
cd D:\
git clone https://github.com/benwolverinee/AetherNet-Releases.git
```

### 4. İlk version.json Oluştur

```bash
cd D:\AetherNet-Releases
echo { > version.json
echo   "Version": "1.0", >> version.json
echo   "DownloadUrl": "https://github.com/benwolverinee/AetherNet-Releases/releases/latest/download/AetherNet.exe" >> version.json
echo } >> version.json

git add version.json
git commit -m "Initial version"
git push
```

### 5. GitHub CLI Kur (Opsiyonel ama Önerilen)

1. İndir: https://cli.github.com/
2. Kur
3. Terminal'de:
```bash
gh auth login
```

## Güncelleme Yayınlama

### Yöntem 1: Otomatik (GitHub CLI ile) ⭐

```bash
release-auto.bat
```

1. Yeni version gir (örn: 1.2)
2. Script her şeyi yapar:
   - Build
   - Private repo'ya push
   - Public repo'ya version.json push
   - GitHub Release oluşturur
   - Exe'yi yükler

✅ Tamamen otomatik!

### Yöntem 2: Manuel (GitHub CLI olmadan)

```bash
release-public.bat
```

1. Yeni version gir
2. Script build yapar ve talimatları gösterir
3. Manuel adımları takip et:

```bash
# version.json'u public repo'ya yükle
cd D:\AetherNet-Releases
git add version.json
git commit -m "Update to v1.2"
git push

# GitHub Release oluştur
# https://github.com/benwolverinee/AetherNet-Releases/releases/new
# Tag: v1.2
# Title: AetherNet v1.2
# Upload: releases\v1.2\AetherNet.exe
```

## Kullanıcılar Nasıl İndirir?

İndirme linki:
```
https://github.com/benwolverinee/AetherNet-Releases/releases/latest/download/AetherNet.exe
```

Veya releases sayfası:
```
https://github.com/benwolverinee/AetherNet-Releases/releases
```

## Otomatik Güncelleme Nasıl Çalışır?

1. Uygulama başladığında `AutoUpdater.cs` çalışır
2. `https://raw.githubusercontent.com/benwolverinee/AetherNet-Releases/main/version.json` kontrol eder
3. Yeni version varsa kullanıcıya sorar
4. Kabul ederse exe'yi indirir ve günceller
5. Uygulamayı yeniden başlatır

## Sorun Giderme

### "gh: command not found"
GitHub CLI yüklü değil. `release-public.bat` kullan (manuel yöntem).

### Exe dosyası 100 MB'dan büyük
Normal. GitHub Releases kullanıyoruz (2 GB'a kadar destekler). Git push ile yükleme.

### Güncelleme çalışmıyor
- `version.json` public repo'da main branch'te olmalı
- GitHub Release yayınlanmış olmalı
- URL'ler doğru olmalı

### Build hatası
- .NET 8.0 SDK yüklü olmalı
- Native dosyalar (goodbyedpi.exe, WinDivert.dll, WinDivert64.sys) mevcut olmalı

### Kullanıcılar kodu görebilir mi?
Hayır. Kaynak kod private repo'da. Kullanıcılar sadece public repo'daki exe'yi indirebilir.

## Test

Güncelleme sistemini test etmek için:

1. `release-auto.bat` çalıştır
2. Yeni version gir (örn: 1.2)
3. GitHub Release'in oluşmasını bekle
4. Eski versiyonu çalıştır
5. Güncelleme bildirimi gelecek!

## Detaylı Bilgi

Daha fazla bilgi için: `RELEASE_GUIDE.md`
