@echo off
echo Quick Build...

REM Sadece degisen dosyalari build et
dotnet build AetherNet\AetherNet.csproj -c Release --no-incremental /p:UseSharedCompilation=true /m

echo.
echo Build tamamlandi: AetherNet\bin\Release\net8.0-windows\win-x64\
echo.
pause
