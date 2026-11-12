@echo off
REM Build.bat - Buildira NetGenDLL.vbproj i kopira DLL na traženu lokaciju

setlocal enabledelayedexpansion

REM Putanje
set MSBUILD="C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe"
set PROJECT_PATH=%~dp0NetGenDLL.vbproj
set SOURCE_DLL=%~dp0bin\Debug\NetGenDLL.dll
set DEST_PATH="C:\Program Files (x86)\Jupiter Software\Jupiter Client\Programs\SharedAssemblies\Spin"
set LAUNCHER_PATH="C:\Users\kkovac\source\repos\NetGen2026 - razni pokušaji i verzije\Lanucher\Launcher\bin\Debug"

echo.
echo ===============================================
echo NetGenDLL Build i Deploy
echo ===============================================
echo.
echo Pocinje build...
echo.

REM Izvršavanje builda
%MSBUILD% "%PROJECT_PATH%" /t:Rebuild /p:Configuration=Debug

REM Provjera je li build bio uspješan
if %ERRORLEVEL% NEQ 0 (
    echo.
    echo [ERROR] Build je neuspješan! Greška kod: %ERRORLEVEL%
    echo.
    pause
    exit /b 1
)

echo.
echo Build je uspješan!
echo.
echo Kopiranje DLL datoteke...
echo.

REM Provjera postoji li source DLL
if not exist "%SOURCE_DLL%" (
    echo.
    echo [ERROR] DLL datoteka nije pronađena na: %SOURCE_DLL%
    echo.
    pause
    exit /b 1
)

REM Provjera postoji li destination direktorij
if not exist %DEST_PATH% (
    echo.
    echo [ERROR] Destination direktorij nije pronađen: %DEST_PATH%
    echo.
    pause
    exit /b 1
)

REM Kopiranje datoteke
copy "%SOURCE_DLL%" %DEST_PATH%


REM Provjera je li kopiranje bilo uspješno
if %ERRORLEVEL% NEQ 0 (
    echo.
    echo [ERROR] Kopiranje je neuspješno! Greška kod: %ERRORLEVEL%
    echo.
    pause
    exit /b 1
)


copy "%SOURCE_DLL%" %LAUNCHER_PATH%


echo.
echo ===============================================
echo USPJEŠNO!
echo ===============================================
echo DLL je kopirana na: %DEST_PATH%
echo.
pause
exit /b 0
