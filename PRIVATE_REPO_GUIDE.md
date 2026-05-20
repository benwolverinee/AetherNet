# Private Repository Kullanımı

## Kodları Gizlemek İçin

### 1. Repository'yi Private Yap

1. https://github.com/benwolverinee/AetherNet/settings
2. **Danger Zone** → **Change visibility**
3. **Make private** → Şifreyi gir → Onayla

✅ Artık sadece sen görebilirsin!

### 2. GitHub Actions'ı Kaldır (Opsiyonel)

Private repo'da GitHub Actions limiti var (2000 dakika/ay). Manuel release yapacağız:

```bash
# .github klasörünü sil
rmdir /s /q .github
git add .
git commit -m "Remove GitHub Actions"
git push
```

## Release Yayınlama (Manuel)

### Yöntem 1: release.bat (Kolay)

```bash
release.bat
```

1. Yeni version gir (örn: 1.0.1)
2. Otomatik build yapar
3. Git commit/push yapar
4. GitHub'da manuel release oluştur

### Yöntem 2: GitHub CLI (Tam Otomatik)

#### GitHub CLI Kurulum

1. İndir: https://cli.github.com/
2. Kur
3. Terminal'de: `gh auth login`

#### Release Oluştur

```bash
# Version'ı güncelle
# version.json: 1.0.0 → 1.0.1
# AetherNet.csproj: <Version>1.0.0</Version> → <Version>1.0.1</Version>

# Build yap
publish.bat

# Release oluştur
gh release create v1.0.1 publish\AetherNet.exe --title "AetherNet v1.0.1" --notes "Discord bağlantı iyileştirmesi"

# Git push
git add .
git commit -m "v1.0.1"
git push
```

## Kullanıcılar Nasıl İndirir?

Private repo olsa bile **release'ler public olabilir**!

### Release'leri Public Yap

1. Her release oluştururken **public** olarak yayınla
2. Kullanıcılar sadece `.exe` indirir
3. Kod göremezler!

**Veya:**

Release'leri kendi sunucunda host et:
- Google Drive
- Dropbox
- Kendi web sitesi

`version.json`'daki URL'i değiştir:
```json
{
  "Version": "1.0.1",
  "DownloadUrl": "https://drive.google.com/uc?export=download&id=DOSYA_ID"
}
```

## Özet

**Private Repo:**
- ✅ Kod gizli
- ✅ Sadece sen görebilirsin
- ❌ GitHub Actions limiti var

**Public Repo:**
- ❌ Kod görünür
- ✅ Sınırsız GitHub Actions
- ✅ Otomatik build

**Önerim:** 
- Private yap
- GitHub CLI ile manuel release
- Veya release'leri Google Drive'da host et
