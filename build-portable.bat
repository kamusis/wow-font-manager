@echo off
REM Build script for WoW Font Manager - Portable Release (Windows)
REM Usage: build-portable.bat [win-x64|all] [clean]

setlocal enabledelayedexpansion

set PLATFORM=%1
if "%PLATFORM%"=="" set PLATFORM=win-x64

set PROJECT_PATH=%~dp0src\WowFontManager.csproj
set OUTPUT_BASE=%~dp0publish

echo ========================================
echo WoW Font Manager - Portable Build
echo ========================================
echo.

REM Clean if requested
if /I "%2"=="clean" (
    if exist "%OUTPUT_BASE%" (
        echo Cleaning previous builds...
        rmdir /s /q "%OUTPUT_BASE%"
        echo [OK] Cleaned
        echo.
    )
)

REM Build function
if /I "%PLATFORM%"=="all" (
    call :BuildPlatform win-x64
    call :BuildPlatform osx-x64
    call :BuildPlatform osx-arm64
    call :BuildPlatform linux-x64
) else (
    call :BuildPlatform %PLATFORM%
)

echo ========================================
echo Build Complete!
echo ========================================
echo.
echo Output location: %OUTPUT_BASE%
echo.
echo Usage:
echo   1. Copy the executable to any directory
echo   2. Create a 'fonts' folder and add your font files
echo   3. Run WowFontManager and enjoy!
echo.
goto :eof

:BuildPlatform
set RID=%1
set OUTPUT_PATH=%OUTPUT_BASE%\%RID%

echo Building for %RID%...
echo Output: %OUTPUT_PATH%
echo.

dotnet publish "%PROJECT_PATH%" ^
    --configuration Release ^
    --runtime %RID% ^
    --output "%OUTPUT_PATH%" ^
    --self-contained true ^
    /p:PublishSingleFile=true ^
    /p:IncludeNativeLibrariesForSelfExtract=true ^
    /p:DebugType=None ^
    /p:DebugSymbols=false ^
    /p:EnableCompressionInSingleFile=true

if %ERRORLEVEL% equ 0 (
    echo [OK] Build successful for %RID%
    
    REM Show file size
    if "%RID:~0,3%"=="win" (
        set EXE_NAME=WowFontManager.exe
    ) else (
        set EXE_NAME=WowFontManager
    )
    
    if exist "%OUTPUT_PATH%\!EXE_NAME!" (
        for %%F in ("%OUTPUT_PATH%\!EXE_NAME!") do (
            set /a SIZE_MB=%%~zF/1048576
            echo   File: !EXE_NAME!
            echo   Size: !SIZE_MB! MB
        )
    )
    echo.
) else (
    echo [ERROR] Build failed for %RID%
    exit /b 1
)
goto :eof
