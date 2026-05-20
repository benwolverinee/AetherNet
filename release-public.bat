@echo off
echo ========================================
echo AetherNet Public Release
echo ========================================
echo.

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
xcopy "publish\Native" "releases\v%NEW_VERSION%\Native\" /E /I /Y
copy "publish\AetherNet.Service.exe" "releases\v%NEW_VERSION%\" /Y

REM README olustur
echo # AetherNet v%NEW_VERSION% > "releases\v%NEW_VERSION%\README.md"
echo. >> "releases\v%NEW_VERSION%\README.md"
echo Discord, Telegram ve engellenmiş siteler için DPI bypass aracı. >> "releases\v%NEW_VERSION%\README.md"
echo. >> "releases\v%NEW_VERSION%\README.md"
echo ## Kurulum >> "releases\v%NEW_VERSION%\README.md"
echo. >> "releases\v%NEW_VERSION%\README.md"
echo 1. AetherNet.exe'yi indir >> "releases\v%NEW_VERSION%\README.md"
echo 2. Yönetici olarak çalıştır >> "releases\v%NEW_VERSION%\README.md"
echo 3. "Servis Yükle" butonuna tıkla >> "releases\v%NEW_VERSION%\README.md"
echo 4. "BAŞLAT" butonuna tıkla >> "releases\v%NEW_VERSION%\README.md"

REM Private repo'ya commit
git add .
git commit -m "Release v%NEW_VERSION%"
git push

echo.
echo ========================================
echo TAMAMLANDI!
echo ========================================
echo.
echo Release dosyalari: releases\v%NEW_VERSION%\
echo.
echo SIMDI:
echo 1. GitHub'da yeni PUBLIC repository olustur: AetherNet-Releases
echo 2. releases\v%NEW_VERSION% klasorunu oraya yukle
echo.
echo Veya GitHub CLI ile:
echo gh repo create benwolverinee/AetherNet-Releases --public
echo cd releases\v%NEW_VERSION%
echo git init
echo git add .
echo git commit -m "Release v%NEW_VERSION%"
echo git branch -M main
echo git remote add origin https://github.com/benwolverinee/AetherNet-Releases.git
echo git push -u origin main
echo.
pause
