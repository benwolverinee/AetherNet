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

dotnet publish AetherNet.Service\AetherNet.Service.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o publish

REM Release klasoru olustur
if not exist "releases" mkdir "releases"
if not exist "releases\v%NEW_VERSION%" mkdir "releases\v%NEW_VERSION%"

REM Tum dosyalari kopyala
xcopy "publish\*.*" "releases\v%NEW_VERSION%\" /E /I /Y

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

REM Private repo'ya commit (sadece kod)
git add .
git commit -m "Release v%NEW_VERSION%"
git push

echo.
echo ========================================
echo PUBLIC RELEASE HAZIRLANIYOR...
echo ========================================

REM Public releases repo icin version.json olustur
if not exist "D:\AetherNet-Releases" mkdir "D:\AetherNet-Releases"

echo { > "D:\AetherNet-Releases\version.json"
echo   "Version": "%NEW_VERSION%", >> "D:\AetherNet-Releases\version.json"
echo   "DownloadUrl": "https://github.com/benwolverinee/AetherNet-Releases/releases/latest/download/AetherNet.exe" >> "D:\AetherNet-Releases\version.json"
echo } >> "D:\AetherNet-Releases\version.json"

echo.
echo ========================================
echo TAMAMLANDI!
echo ========================================
echo.
echo Build dosyalari: releases\v%NEW_VERSION%\
echo Public repo dosyasi: D:\AetherNet-Releases\version.json
echo.
echo SIMDI YAPILACAKLAR:
echo.
echo 1. Public repo'ya version.json yukle:
echo    cd D:\AetherNet-Releases
echo    git add version.json
echo    git commit -m "Update to v%NEW_VERSION%"
echo    git push
echo.
echo 2. GitHub Release olustur (exe 100MB'dan buyuk, git push calismaz):
echo    Tum klasoru zip'le: releases\v%NEW_VERSION%
echo    gh release create v%NEW_VERSION% "releases\v%NEW_VERSION%.zip" --repo benwolverinee/AetherNet-Releases --title "AetherNet v%NEW_VERSION%" --notes "Discord ve engellenmiş siteler için DPI bypass"
echo.
echo VEYA manuel:
echo    - releases\v%NEW_VERSION% klasorunu zip'le
echo    - https://github.com/benwolverinee/AetherNet-Releases/releases/new
echo    - Tag: v%NEW_VERSION%
echo    - Title: AetherNet v%NEW_VERSION%
echo    - Upload: releases\v%NEW_VERSION%.zip
echo.
pause
