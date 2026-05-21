@echo off
echo ========================================
echo AetherNet Build
echo ========================================
echo.

REM Eski dosyalari temizle
if exist "publish" rmdir /s /q "publish"
if exist "releases\AetherNet.zip" del "releases\AetherNet.zip"

REM Build - Paralel ve hizli
echo Build yapiliyor...
dotnet build AetherNet.Service\AetherNet.Service.csproj -c Release --no-incremental
dotnet publish AetherNet\AetherNet.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:PublishReadyToRun=false -o publish --no-restore

REM Native dosyalari kopyala
xcopy "AetherNet\Native\*" "publish\Native\" /E /I /Y >nul

REM Service build
dotnet publish AetherNet.Service\AetherNet.Service.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o publish --no-restore

REM Zip olustur
echo.
echo Zip olusturuluyor...
if not exist "releases" mkdir "releases"
powershell -Command "Compress-Archive -Path 'publish\*' -DestinationPath 'releases\AetherNet.zip' -Force"

echo.
echo ========================================
echo TAMAMLANDI!
echo ========================================
echo.
echo Zip: releases\AetherNet.zip
echo.
pause
