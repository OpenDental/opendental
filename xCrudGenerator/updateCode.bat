@echo off

echo Updating code
REM Update Code. If you have repositories in other directories, include them in the /path argument separated by a '*'.
TortoiseProc.exe /command:update /path:"C:\development\OPEN DENTAL SUBVERSION*C:\development\Shared Projects Subversion" /closeonend:2

echo Building Crud Generator project
REM Build xCrudGenerator project (this will also build OpenDentBusiness)
"C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\devenv.exe" /build Debug "C:\development\OPEN DENTAL SUBVERSION\head\xCrudGenerator\xCrudGenerator.csproj"

echo Updating databases
REM Update databases. If you have more databases that you want to keep current, duplicate the following line and change the database name. You can optionally specify the server, user, and password.
"C:\development\OPEN DENTAL SUBVERSION\head\xCrudGenerator\bin\Debug\CrudGenerator.exe" database=development172 doUpdateDb=true

echo Done updating code and databases
timeout /t 10