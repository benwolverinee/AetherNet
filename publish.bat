@echo off
echo ========================================
echo AetherNet Tek EXE Olusturuluyor...
echo ========================================

REM Eski publish klasorunu temizle
if exist "publish" rmdir /s /q "publish"

REM Tek exe olustur
dotnet publish AetherNet\AetherNet.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o publish

REM Native dosyalari kopyala (goodbyedpi.exe, WinDivert.dll, WinDivert64.sys)
mkdir "publish\Native"
copy "AetherNet\Native\goodbyedpi.exe" "publish\Native\" /Y
copy "AetherNet\Native\WinDivert.dll" "publish\Native\" /Y
copy "AetherNet\Native\WinDivert64.sys" "publish\Native\" /Y

REM Servis dosyalarini kopyala
dotnet publish AetherNet.Service\AetherNet.Service.csproj -c Release -r win-x64 --self-contained false -o publish

echo.
echo ========================================
echo TAMAMLANDI!
echo ========================================
echo.
echo Dosyalar: publish\AetherNet.exe
echo.
echo GitHub'a yuklemek icin:
echo 1. GitHub repository olustur
echo 2. version.json dosyasini repository'ye yukle
echo 3. Release olustur ve AetherNet.exe'yi yukle
echo 4. AutoUpdater.cs'deki YOUR_USERNAME'i degistir
echo.
pause
