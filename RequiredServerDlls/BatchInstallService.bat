@echo off
echo installutil.exe OpenDentServer.exe
echo.
pushd "%~dp0"
installutil.exe OpenDentServer.exe
popd
echo.
pause
