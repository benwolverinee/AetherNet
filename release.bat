@echo off
echo ========================================
echo AetherNet Release Olusturuluyor
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

REM Eski publish klasorunu temizle
if exist "publish" rmdir /s /q "publish"

REM Tek exe olustur
dotnet publish AetherNet\AetherNet.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o publish

REM Native dosyalari kopyala
mkdir "publish\Native"
copy "AetherNet\Native\goodbyedpi.exe" "publish\Native\" /Y
copy "AetherNet\Native\WinDivert.dll" "publish\Native\" /Y
copy "AetherNet\Native\WinDivert64.sys" "publish\Native\" /Y

REM Servis dosyalarini kopyala
dotnet publish AetherNet.Service\AetherNet.Service.csproj -c Release -r win-x64 --self-contained false -o publish

REM Release klasoru olustur
if not exist "releases" mkdir "releases"
if not exist "releases\v%NEW_VERSION%" mkdir "releases\v%NEW_VERSION%"

REM Dosyalari kopyala
copy "publish\AetherNet.exe" "releases\v%NEW_VERSION%\" /Y
xcopy "publish\Native" "releases\v%NEW_VERSION%\Native\" /E /I /Y
copy "publish\AetherNet.Service.exe" "releases\v%NEW_VERSION%\" /Y

REM Git commit
git add .
git commit -m "Release v%NEW_VERSION%"
git tag -a "v%NEW_VERSION%" -m "Release v%NEW_VERSION%"
git push origin main
git push origin "v%NEW_VERSION%"

echo.
echo ========================================
echo TAMAMLANDI!
echo ========================================
echo.
echo Release: releases\v%NEW_VERSION%\AetherNet.exe
echo.
echo Simdi yapmaniz gerekenler:
echo 1. GitHub'da Releases sayfasina git
echo 2. "Draft a new release" tikla
echo 3. Tag: v%NEW_VERSION% sec
echo 4. Title: AetherNet v%NEW_VERSION%
echo 5. releases\v%NEW_VERSION%\AetherNet.exe dosyasini yukle
echo 6. Publish release tikla
echo.
echo Veya otomatik GitHub Release olusturmak icin:
echo gh release create v%NEW_VERSION% releases\v%NEW_VERSION%\AetherNet.exe --title "AetherNet v%NEW_VERSION%" --notes "Yeni versiyon"
echo.
pause
