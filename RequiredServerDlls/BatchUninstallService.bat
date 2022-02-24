@echo off
echo installutil.exe /u OpenDentServer.exe
echo.
pushd "%~dp0"
installutil.exe /u OpenDentServer.exe
popd
echo.
pause
