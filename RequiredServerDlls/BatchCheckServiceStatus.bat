@echo off
echo This script attempts to pause the OpenDental service.
echo Pausing is not supported, but the error message is useful.
echo It will tell you whether the service exists,
echo and whether it's running or not. 
echo.
net pause OpenDental
echo.
pause
