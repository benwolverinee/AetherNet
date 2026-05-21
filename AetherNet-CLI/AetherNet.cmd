@echo off
chcp 65001 >nul
setlocal enabledelayedexpansion
cd /d "%~dp0"

for /F %%a in ('echo prompt $E ^| cmd') do set "ESC=%%a"

net session >nul 2>&1
if %errorlevel% neq 0 (
    call :yellow "Yonetici yetkisi gerekli..."
    powershell -Command "Start-Process '%~f0' -Verb RunAs"
    exit /b
)

title AetherNet
mode con: cols=60 lines=34
color 0F

:: =========================
:: ILK ACILISTA DISCLAIMER
:: =========================
if not exist "%~dp0.accepted" goto disclaimer
goto menu

:disclaimer
cls
call :cyan "═══════════════════════════════════════════════════════════"
call :red  "                YASAL UYARI / SORUMLULUK REDDI"
call :cyan "═══════════════════════════════════════════════════════════"
echo.
echo   AetherNet, yalnizca egitim ve kisisel ag testi
echo   amaciyla gelistirilmis bir yazilimdir.
echo.
call :yellow "   - Bu araci kullanmak tamamen KULLANICININ"
call :yellow "     kendi sorumlulugundadir."
echo.
call :yellow "   - Gelistirici ( benwolverinee ), olusabilecek"
call :yellow "     hicbir zarardan sorumlu degildir."
echo.
call :cyan "───────────────────────────────────────────────────────────"
echo.
echo   Devam ederseniz yukaridaki sartlari OKUDUGUNUZU
echo   ve KABUL ETTIGINIZI beyan etmis sayilirsiniz.
echo.
call :cyan "───────────────────────────────────────────────────────────"
echo.
set /p accept="  Kabul ediyor musunuz? (E/H): "

if /I "%accept%"=="E" (
    echo accepted > "%~dp0.accepted"
    goto menu
)
exit

:: =========================
:: DOSYA YOLU KONTROLU
:: =========================
if not exist "%~dp0bin\goodbyedpi.exe" (
    cls
    call :red "═══════════════════════════════════════════════"
    call :red "              DOSYA EKSIK HATASI"
    call :red "═══════════════════════════════════════════════"
    echo.
    call :yellow "  bin klasoru veya icindeki dosyalar eksik!"
    echo.
    pause
    exit
)

:menu
cls

call :cyan "═══════════════════════════════════════════════"
call :cyan "                 benwolverinee"
call :cyan "═══════════════════════════════════════════════"
echo.

set "STATE="

sc query GoodbyeDPI >nul 2>&1
if %errorlevel% neq 0 (
    call :red "   Servis Durumu : YUKLU DEGIL"
    call :red "   Sistem Durumu : KAPALI"
) else (
    for /f "tokens=4" %%a in ('sc query GoodbyeDPI ^| findstr /C:"STATE"') do set "STATE=%%a"

    if /I "!STATE!"=="RUNNING" (
        call :green "   Servis Durumu : YUKLU"
        call :green "   Sistem Durumu : AKTIF"
    ) else if /I "!STATE!"=="STOPPED" (
        call :yellow "   Servis Durumu : YUKLU"
        call :yellow "   Sistem Durumu : PASIF"
    ) else (
        call :yellow "   Servis Durumu : YUKLU"
        call :yellow "   Sistem Durumu : DEGISTIRILIYOR"
    )
)

echo.
call :cyan "───────────────────────────────────────────────"
echo.

echo   BILGILENDIRME:
echo   - Servis kurulduktan sonra otomatik BASLATILIR
echo   - Durum yukarida servis altinda gorunur
echo   - Baslat / Durdur anlik calisir
echo   - Kaldirma servisi tamamen siler
echo.

echo   KULLANIM:
echo   [1] Servisi Yukle
echo   [2] Servisi Kaldir
echo   [3] Sistemi Baslat
echo   [4] Sistemi Durdur
echo   [5] Cikis
echo.

call :cyan "───────────────────────────────────────────────"
echo   Oneri / Destek / Sosyal Medya
call :cyan "───────────────────────────────────────────────"

call :line "  Instagram : "  "@berkaysunanofficial"    95
call :line "  TikTok    : "  "@benwolverinee"          94
call :line "  YouTube   : "  "@berkaysunn"             91
call :line "  YouTube   : "  "@benwolverinee"          91
call :line "  Tüm Link  : "  "linktr.ee/benwolverinee" 0

call :cyan "───────────────────────────────────────────────"
echo.

set /p choice="  Seciminiz (1-5): "

if "%choice%"=="1" goto install
if "%choice%"=="2" goto uninstall
if "%choice%"=="3" goto startsvc
if "%choice%"=="4" goto stopsvc
if "%choice%"=="5" goto showdisclaimer
if "%choice%"=="6" exit

goto menu

:startsvc
sc start GoodbyeDPI >nul 2>&1
goto menu

:stopsvc
sc stop GoodbyeDPI >nul 2>&1
goto menu

:install
sc query GoodbyeDPI >nul 2>&1
if %errorlevel% equ 0 goto menu

(
    echo discord.com
    echo discordapp.com
    echo discord.gg
    echo cdn.discordapp.com
) > "%~dp0list.txt"

sc create GoodbyeDPI binPath= "\"%~dp0bin\goodbyedpi.exe\" -5 --set-ttl 5 --blacklist \"%~dp0list.txt\"" DisplayName= "AetherNet DPI" start= auto >nul 2>&1
sc description GoodbyeDPI "AetherNet DPI Bypass Service" >nul 2>&1
sc failure GoodbyeDPI reset= 0 actions= restart/5000/restart/5000/restart/5000 >nul 2>&1
sc start GoodbyeDPI >nul 2>&1

goto menu

:uninstall
sc stop GoodbyeDPI >nul 2>&1
sc delete GoodbyeDPI >nul 2>&1
goto menu

:: =========================
:: COLORS
:: =========================
:green
echo %ESC%[92m%~1%ESC%[0m
exit /b

:red
echo %ESC%[91m%~1%ESC%[0m
exit /b

:yellow
echo %ESC%[93m%~1%ESC%[0m
exit /b

:cyan
echo %ESC%[96m%~1%ESC%[0m
exit /b

:line
<nul set /p "=%~1"
echo %ESC%[%~3m%~2%ESC%[0m
exit /b