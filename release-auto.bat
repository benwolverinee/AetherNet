@echo off
echo ========================================
echo AetherNet Otomatik Release
echo ========================================
echo.

REM GitHub CLI kontrolu
where gh >nul 2>nul
if %ERRORLEVEL% NEQ 0 (
    echo HATA: GitHub CLI yuklu degil!
    echo.
    echo Yuklemek icin: https://cli.github.com/
    echo.
    echo Veya release-public.bat kullan (manuel)
    pause
    exit /b 1
)

REM Version'u oku
for /f "tokens=2 delims=:, " %%a in ('findstr /C:"Version" version.json') do set VERSION=%%~a
set VERSION=%VERSION:"=%

echo Mevcut Version: %VERSION%
echo.
set /p NEW_VERSION="Yeni version girin (ornek: 1.0.1): "

REM version.json'u guncelle
powershell -Command "(Get-Content 'version.json') -replace '\"Version\": \"%VERSION%\"', '\"Version\": \"%NEW_VERSION%\"' | Set-Content 'version.json'"

REM AetherNet.csproj'u guncelle
powershell -Command "(Get-Content 'AetherNet\AetherNet.csproj') -replace '<Version>%VERSION%</Version>', '<Version>%NEW_VERSION%</Version>' | Set-Content 'AetherNet\AetherNet.csproj'"

echo.
echo Version guncellendi: %VERSION% -^> %NEW_VERSION%
echo.

REM Build yap
echo ========================================
echo Build yapiliyor...
echo ========================================

if exist "publish" rmdir /s /q "publish"

dotnet publish AetherNet\AetherNet.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o publish

mkdir "publish\Native"
copy "AetherNet\Native\goodbyedpi.exe" "publish\Native\" /Y
copy "AetherNet\Native\WinDivert.dll" "publish\Native\" /Y
copy "AetherNet\Native\WinDivert64.sys" "publish\Native\" /Y

dotnet publish AetherNet.Service\AetherNet.Service.csproj -c Release -r win-x64 --self-contained false -o publish

REM Release klasoru olustur
if not exist "releases" mkdir "releases"
if not exist "releases\v%NEW_VERSION%" mkdir "releases\v%NEW_VERSION%"

copy "publish\AetherNet.exe" "releases\v%NEW_VERSION%\" /Y

REM Private repo'ya commit
echo.
echo ========================================
echo Private repo guncelleniyor...
echo ========================================
git add .
git commit -m "Release v%NEW_VERSION%"
git push

REM Public releases repo icin version.json olustur
echo.
echo ========================================
echo Public repo hazirlaniyor...
echo ========================================

if not exist "D:\AetherNet-Releases" mkdir "D:\AetherNet-Releases"

echo { > "D:\AetherNet-Releases\version.json"
echo   "Version": "%NEW_VERSION%", >> "D:\AetherNet-Releases\version.json"
echo   "DownloadUrl": "https://github.com/benwolverinee/AetherNet-Releases/releases/latest/download/AetherNet.exe" >> "D:\AetherNet-Releases\version.json"
echo } >> "D:\AetherNet-Releases\version.json"

REM Public repo'ya version.json yukle
cd D:\AetherNet-Releases
git add version.json
git commit -m "Update to v%NEW_VERSION%"
git push

REM GitHub Release olustur ve exe yukle
echo.
echo ========================================
echo GitHub Release olusturuluyor...
echo ========================================

cd %~dp0
gh release create v%NEW_VERSION% "releases\v%NEW_VERSION%\AetherNet.exe" --repo benwolverinee/AetherNet-Releases --title "AetherNet v%NEW_VERSION%" --notes "Discord, Telegram ve engellenmiş siteler için DPI bypass aracı.%0A%0AKurulum:%0A1. AetherNet.exe'yi indir%0A2. Yönetici olarak çalıştır%0A3. 'Servis Yükle' butonuna tıkla%0A4. 'BAŞLAT' butonuna tıkla"

echo.
echo ========================================
echo TAMAMLANDI!
echo ========================================
echo.
echo Private repo: https://github.com/benwolverinee/AetherNet
echo Public releases: https://github.com/benwolverinee/AetherNet-Releases/releases
echo.
echo Kullanicilar buradan indirecek:
echo https://github.com/benwolverinee/AetherNet-Releases/releases/latest/download/AetherNet.exe
echo.
pause
