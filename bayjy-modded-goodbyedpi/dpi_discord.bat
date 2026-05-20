@echo off
chcp 65001 > nul
color 4
@REM mode 1500
SETLOCAL EnableDelayedExpansion
Title BayJiy dc bypass kurulumu

echo. BBBBBBBBBBBBBBBBB                                                      JJJJJJJJJJJ   iiii                          
echo. B::::::::::::::::B                                                     J:::::::::J  i::::i                         
echo. B::::::BBBBBB:::::B                                                    J:::::::::J   iiii                          
echo. BB:::::B     B:::::B                                                   JJ:::::::JJ                               
echo.   B::::B     B:::::B    aaaaaaaaaaaaa  yyyyyyy           yyyyyyy         J:::::J   iiiiiiii  yyyyyyy         yyyyyyy
echo.   B::::B     B:::::B    a::::::::::::a  y:::::y         y:::::y          J:::::J   i::::::i  y:::::y         y:::::y 
echo.   B::::BBBBBB:::::B     aaaaaaaaa:::::a  y:::::y       y:::::y           J:::::J    i::::i    y:::::y       y:::::y  
echo.   B:::::::::::::BB               a::::a   y:::::y     y:::::y            J:::::j    i::::i     y:::::y     y:::::y   
echo.   B::::BBBBBB:::::B       aaaaaaa:::::a    y:::::y   y:::::y             J:::::J    i::::i      y:::::y   y:::::y    
echo.   B::::B     B:::::B    aa::::::::::::a     y:::::y y:::::y  JJJJJJJ     J:::::J    i::::i       y:::::y y:::::y     
echo.   B::::B     B:::::B   a::::aaaa::::::a      y:::::y:::::y   J:::::J     J:::::J    i::::i        y:::::y:::::y      
echo.   B::::B     B:::::B  a::::a    a:::::a       y:::::::::y    J::::::J   J::::::J    i::::i         y:::::::::y       
echo. BB:::::BBBBBB::::::B  a::::a    a:::::a        y:::::::y     J:::::::JJJ:::::::J   i::::::i         y:::::::y        
echo. B:::::::::::::::::B   a:::::aaaa::::::a         y:::::y       JJ:::::::::::::JJ    i::::::i          y:::::y         
echo. B::::::::::::::::B     a::::::::::aa:::a       y:::::y          JJ:::::::::JJ      i::::::i         y:::::y          
echo. BBBBBBBBBBBBBBBBB       aaaaaaaaaa  aaaa      y:::::y             JJJJJJJJJ        iiiiiiii        y:::::y           
echo.                                              y:::::y                                              y:::::y            
echo.                                             y:::::y                                              y:::::y             
echo.                                            y:::::y                                              y:::::y              
echo.                                           y:::::y                                              y:::::y               
echo.                                          yyyyyyy                                              yyyyyyy                
echo. 							
echo.                                     Bayjiy dc bypass servisi ekleniyor...
Echo	Dosyayı yönetici olarak çalıştırmayı unutmayın...
Echo	Dosyayı yönetici olarak çalıştırmazsanız çalışmayacaktır...

timeout /t 3 /nobreak  >nul 2>&1

PUSHD "%~dp0"
set _arch=x86
IF "%PROCESSOR_ARCHITECTURE%"=="AMD64" (set _arch=x86_64)
IF DEFINED PROCESSOR_ARCHITEW6432 (set _arch=x86_64)

sc stop "GoodbyeDPI"
sc delete "GoodbyeDPI"
sc create "bayjiydcbypass" binPath= "\"%CD%\%_arch%\goodbyedpi.exe\" -5 --blacklist \"%CD%\list.txt\" --dns-addr 77.88.8.8 --dns-port 1253 --dnsv6-addr 2a02:6b8::feed:0ff --dnsv6-port 1253" start= "auto"
sc description "bayjiydcbypass" "Sadece discord goodbye dpi modded by bayjiy"
sc start "bayjiydcbypass"
::dns
set INTERFACE=

set DNS1=1.1.1.1
set DNS2=1.0.0.1

for /f "tokens=3,*" %%i in ('netsh int show interface ^| find "Connected"') do set INTERFACE=%%j

netsh int ipv4 set dns name="%INTERFACE%" static %DNS1% primary
if defined DNS2 netsh int ipv4 add dns name="%INTERFACE%" %DNS2% index=2

echo İşlem Tamam...

timeout /t 5 /nobreak  >nul 2>&1
start "" "https://dsc.gg/bayjiy"

