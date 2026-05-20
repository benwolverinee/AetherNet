@echo off
echo ========================================
echo GitHub Otomatik Guncelleme Kurulumu
echo ========================================
echo.

set /p username="GitHub kullanici adinizi girin: "

echo.
echo AutoUpdater.cs dosyasi guncelleniyor...

powershell -Command "(Get-Content 'AetherNet\Core\AutoUpdater.cs') -replace 'YOUR_USERNAME', '%username%' | Set-Content 'AetherNet\Core\AutoUpdater.cs'"

echo.
echo version.json dosyasi guncelleniyor...

powershell -Command "(Get-Content 'version.json') -replace 'YOUR_USERNAME', '%username%' | Set-Content 'version.json'"

echo.
echo ========================================
echo TAMAMLANDI!
echo ========================================
echo.
echo Simdi yapmaniz gerekenler:
echo.
echo 1. GitHub'da yeni repository olusturun (ornek: %username%/AetherNet)
echo 2. Bu projeyi GitHub'a yukleyin:
echo    git init
echo    git add .
echo    git commit -m "Initial commit"
echo    git branch -M main
echo    git remote add origin https://github.com/%username%/AetherNet.git
echo    git push -u origin main
echo.
echo 3. Her kod degisikliginde:
echo    - version.json'daki version'i artirin (ornek: 1.0.0 -^> 1.0.1)
echo    - AetherNet.csproj'deki ^<Version^> degerini artirin
echo    - git commit ve git push yapin
echo    - GitHub Actions otomatik build yapacak ve release olusturacak
echo.
echo 4. Kullanicilar AetherNet.exe'yi actiginda otomatik guncelleme alacak!
echo.
pause
