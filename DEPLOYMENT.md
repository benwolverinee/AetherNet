# AetherNet Otomatik Güncelleme Sistemi

## Nasıl Çalışır?

1. **Kod değişikliği yap** → `git push`
2. **GitHub Actions** otomatik build yapar
3. **Release oluşturur** (AetherNet.exe)
4. **Kullanıcılar** uygulamayı açınca otomatik güncelleme alır

## İlk Kurulum

### 1. GitHub Repository Oluştur

```bash
# GitHub'da yeni repository oluştur: AetherNet
```

### 2. Setup Script'ini Çalıştır

```bash
setup-github.bat
```

GitHub kullanıcı adını gir (örnek: `cagritaskn`)

### 3. Projeyi GitHub'a Yükle

```bash
git init
git add .
git commit -m "Initial commit"
git branch -M main
git remote add origin https://github.com/KULLANICI_ADIN/AetherNet.git
git push -u origin main
```

### 4. GitHub Actions İzinlerini Ayarla

1. GitHub repository → Settings → Actions → General
2. "Workflow permissions" → "Read and write permissions" seç
3. "Allow GitHub Actions to create and approve pull requests" işaretle
4. Save

## Güncelleme Yayınlama

### Adım 1: Version'ı Artır

**version.json:**
```json
{
  "Version": "1.0.1",  // 1.0.0 → 1.0.1
  "DownloadUrl": "..."
}
```

**AetherNet/AetherNet.csproj:**
```xml
<Version>1.0.1</Version>  <!-- 1.0.0 → 1.0.1 -->
```

### Adım 2: Commit ve Push

```bash
git add .
git commit -m "v1.0.1: Discord bağlantı iyileştirmesi"
git push
```

### Adım 3: Otomatik Build

- GitHub Actions otomatik çalışır
- Build yapar
- Release oluşturur
- AetherNet.exe yükler

### Adım 4: Kullanıcılar Güncellenir

- Kullanıcılar uygulamayı açınca güncelleme bildirimi alır
- "Evet" derse otomatik güncellenir
- Uygulama yeniden başlar

## Manuel Build (Opsiyonel)

Eğer GitHub Actions kullanmak istemezsen:

```bash
publish.bat
```

Sonra manuel olarak GitHub Release oluştur ve `publish\AetherNet.exe`'yi yükle.

## Güncelleme Akışı

```
Geliştirici                    GitHub                      Kullanıcı
    |                             |                            |
    | git push                    |                            |
    |------------------------->   |                            |
    |                             |                            |
    |                      GitHub Actions                      |
    |                       Build + Release                    |
    |                             |                            |
    |                             |   Uygulama açılır          |
    |                             |<---------------------------|
    |                             |                            |
    |                             |   Güncelleme kontrolü      |
    |                             |--------------------------->|
    |                             |                            |
    |                             |   Yeni versiyon var!       |
    |                             |<---------------------------|
    |                             |                            |
    |                             |   AetherNet.exe indir      |
    |                             |--------------------------->|
    |                             |                            |
    |                             |                    Otomatik güncelle
    |                             |                            |
```

## Sorun Giderme

### GitHub Actions çalışmıyor
- Settings → Actions → General → Workflow permissions kontrol et
- "Read and write permissions" olmalı

### Güncelleme çalışmıyor
- `version.json` GitHub'da main branch'te olmalı
- URL'ler doğru olmalı (YOUR_USERNAME değiştirilmiş olmalı)
- Release oluşturulmuş olmalı

### Build hatası
- .NET 8.0 SDK yüklü olmalı
- Native dosyalar (goodbyedpi.exe, WinDivert.dll, WinDivert64.sys) mevcut olmalı

## Test

Güncelleme sistemini test etmek için:

1. Version'ı artır (1.0.0 → 1.0.1)
2. Push yap
3. GitHub Actions'ın tamamlanmasını bekle
4. Eski versiyonu çalıştır
5. Güncelleme bildirimi gelecek!
