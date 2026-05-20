@echo off
echo ========================================
echo AetherNet-Releases Repo Kurulumu
echo ========================================
echo.

REM D:\AetherNet-Releases klasorunu kontrol et
if not exist "D:\AetherNet-Releases" (
    echo D:\AetherNet-Releases bulunamadi, klonlaniyor...
    git clone https://github.com/benwolverinee/AetherNet-Releases.git D:\AetherNet-Releases
)

REM version.json olustur
echo { > D:\AetherNet-Releases\version.json
echo   "Version": "1.0", >> D:\AetherNet-Releases\version.json
echo   "DownloadUrl": "https://github.com/benwolverinee/AetherNet-Releases/releases/latest/download/AetherNet.exe" >> D:\AetherNet-Releases\version.json
echo } >> D:\AetherNet-Releases\version.json

echo.
echo version.json olusturuldu!
echo.

REM Git push
cd D:\AetherNet-Releases
git add version.json
git commit -m "Initial version.json"
git push

echo.
echo ========================================
echo TAMAMLANDI!
echo ========================================
echo.
echo Simdi release-auto.bat calistir
echo.
pause
