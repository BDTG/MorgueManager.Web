@echo off
echo === MorgueManager.Web - Chay local ===
echo.
echo Dang khoi dong dev server tai http://localhost:5284
echo Mo trinh duyet va truy cap: http://localhost:5284
echo.
dotnet run --project MorgueManager.Web.csproj --launch-profile http
pause
