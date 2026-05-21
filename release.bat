@echo off
echo ========================================
echo AetherNet Release Builder
echo ========================================
echo.

REM Mevcut version'u oku
for /f "tokens=2 delims=:, " %%a in ('findstr /C:"Version" version.json') do set VERSION=%%~a
set VERSION=%VERSION:"=%

echo Mevcut Version: %VERSION%
echo.
set /p NEW_VERSION="Yeni version (ornek: 1.4): "

REM version.json guncelle
powershell -Command "(Get-Content 'version.json') -replace '\"Version\": \"%VERSION%\"', '\"Version\": \"%NEW_VERSION%\"' | Set-Content 'version.json'"

REM AetherNet.csproj guncelle
powershell -Command "(Get-Content 'AetherNet\AetherNet.csproj') -replace '<Version>%VERSION%</Version>', '<Version>%NEW_VERSION%</Version>' | Set-Content 'AetherNet\AetherNet.csproj'"

echo.
echo Version guncellendi: %VERSION% -^> %NEW_VERSION%
echo.

REM Build
echo ========================================
echo Build yapiliyor...
echo ========================================

if exist "publish" rmdir /s /q "publish"

dotnet publish AetherNet\AetherNet.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o publish

xcopy "AetherNet\Native\*" "publish\Native\" /E /I /Y

dotnet publish AetherNet.Service\AetherNet.Service.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o publish

REM Release klasoru
if not exist "releases" mkdir "releases"
if not exist "releases\v%NEW_VERSION%" mkdir "releases\v%NEW_VERSION%"

xcopy "publish\*" "releases\v%NEW_VERSION%\" /E /I /Y

REM Zip olustur
echo.
echo Zip olusturuluyor...
powershell -Command "Compress-Archive -Path 'releases\v%NEW_VERSION%\*' -DestinationPath 'releases\AetherNet-v%NEW_VERSION%.zip' -Force"

REM Git commit
git add .
git commit -m "Release v%NEW_VERSION%"
git push

echo.
echo ========================================
echo TAMAMLANDI!
echo ========================================
echo.
echo Zip dosyasi: releases\AetherNet-v%NEW_VERSION%.zip
echo.
echo SIMDI:
echo 1. GitHub'da Releases'e git
echo 2. "Create a new release" tikla
echo 3. Tag: v%NEW_VERSION%
echo 4. Title: AetherNet v%NEW_VERSION%
echo 5. releases\AetherNet-v%NEW_VERSION%.zip yukle
echo 6. Publish release
echo.
echo Veya GitHub CLI ile:
echo gh release create v%NEW_VERSION% releases\AetherNet-v%NEW_VERSION%.zip --title "AetherNet v%NEW_VERSION%" --notes "Otomatik guncelleme"
echo.
pause
