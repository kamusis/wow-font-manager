# WoW Font Manager - Executable Builds

## How to Run

### Windows (win-x64 folder)
1. Copy the entire `win-x64` folder to your Windows machine
2. Double-click `WowFontManager.exe` to launch the application
3. The application includes all dependencies and doesn't require .NET installation

### macOS (osx-x64 folder)
1. Copy the entire `osx-x64` folder to your Mac
2. Open Terminal and navigate to the folder
3. Run: `chmod +x WowFontManager` (first time only)
4. Run: `./WowFontManager` or double-click the executable
5. The application includes all dependencies and doesn't require .NET installation

## Using the Application

### Main Interface Layout

The UI has three main panels:

**Left Panel - WoW Clients & Fonts**
- Click "Detect Clients" to automatically find installed WoW game clients
- Select a WoW client from the list
- Click "Browse Fonts" to select a directory containing font files
- Available fonts will be listed below

**Center Panel - Font Preview**
- Select a font from the list to see a live preview
- Adjust the preview size using the slider (8-72pt)
- Preview text shows English and Chinese characters to verify font support

**Right Panel - Actions & Backups**
- Click "Replace Fonts" to apply the selected font to the WoW client
- ⚠️ Warning: Always backup before replacing (automatic backup is created)
- View and manage font backups
- Restore previous fonts if needed

### Features

✅ **Auto-Detection**: Automatically finds WoW installations (Retail, Classic, PTR)
✅ **Font Preview**: See exactly how fonts will look before applying
✅ **Safe Replacement**: Creates automatic backups before any changes
✅ **Batch Processing**: Replaces all game fonts (FRIZQT__, ARIALN, etc.) at once
✅ **Backup Management**: Easy restore to previous font configurations
✅ **Cross-Platform**: Works on Windows and macOS

### Supported Font Formats
- .ttf (TrueType Font)
- .otf (OpenType Font)
- .ttc (TrueType Collection)

### Status Bar
- Shows current operation status
- Displays selected WoW client information
- Real-time feedback during font operations

## Troubleshooting

**Windows: "Windows protected your PC" warning**
- Click "More info" → "Run anyway"
- This appears because the executable isn't digitally signed

**macOS: "App can't be opened because it is from an unidentified developer"**
- Right-click the app → Click "Open"
- Or run in Terminal: `xattr -cr WowFontManager`

**No WoW clients detected**
- Manually verify your WoW installation directory
- The app looks in common installation paths
- Make sure you have Fonts folder in your WoW directory

## Technical Details

- Built with .NET 8.0 and Avalonia UI 11.3.6
- Self-contained executables (no .NET runtime installation required)
- Font rendering powered by SkiaSharp
- Cross-platform compatibility

## File Sizes
- Windows executable: ~77 MB
- macOS executable: ~76 MB

These are self-contained builds that include the .NET runtime and all dependencies.
