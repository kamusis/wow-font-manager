@echo off
echo Testing WowFontManager.exe...
echo.
echo Press any key to run the application...
pause >nul

cd publish\win-x64
WowFontManager.exe

echo.
echo Exit code: %ERRORLEVEL%
echo.
echo If the window closed immediately, check the error above.
pause
