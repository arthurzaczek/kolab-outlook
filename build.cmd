@echo off

rem Set Env vars
call "%Program Files%\Microsoft Visual Studio 10.0\vc\bin\vcvars32.bat" x86

rem add mspdb80.dll from vs 2008 to path
set path=%path%;%Program Files%\Microsoft Visual Studio 9.0\Common7\IDE

rem call MSBuild v4
"%windir%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe" OutlookKolab.sln