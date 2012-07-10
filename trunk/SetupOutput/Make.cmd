@echo off
"%VS100COMNTOOLS%\..\IDE\devenv.com" "..\OutlookKolab.sln" /Rebuild "Debug|x86" /Project "OutlookKolabSetup" /ProjectConfig "x86" /log
IF ERRORLEVEL 1 GOTO FAIL
"%VS100COMNTOOLS%\..\IDE\devenv.com" "..\OutlookKolab.sln" /Rebuild "Debug|x64" /Project "OutlookKolabSetup64" /ProjectConfig "x64" /log
IF ERRORLEVEL 1 GOTO FAIL

SET zip=%ProgramFiles%\7-Zip\7z.exe

if exist KolabOutlookSetup86.zip del KolabOutlookSetup86.zip
if exist KolabOutlookSetup64.zip del KolabOutlookSetup64.zip

"%zip%" a -r KolabOutlookSetup86.zip ..\OutlookKolabSetup\x86\*.*
IF ERRORLEVEL 1 GOTO FAIL
"%zip%" a -r KolabOutlookSetup64.zip ..\OutlookKolabSetup64\x64\*.*
IF ERRORLEVEL 1 GOTO FAIL

echo *******************************
echo Finished, now rename both files
echo *******************************
GOTO EOF

:FAIL
echo *******************************
echo            FAILED
echo *******************************

:EOF

pause