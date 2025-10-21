# WoW Font Manager - User Guide

Complete guide for using the WoW Font Manager to customize your World of Warcraft fonts.

## Table of Contents

1. [Getting Started](#getting-started)
2. [Detecting WoW Clients](#detecting-wow-clients)
3. [Browsing Fonts](#browsing-fonts)
4. [Previewing Fonts](#previewing-fonts)
5. [Replacing Fonts](#replacing-fonts)
6. [Restoring Backups](#restoring-backups)
7. [Troubleshooting](#troubleshooting)

## Getting Started

### System Requirements

- **Operating System**: Windows 10/11, macOS 10.15+, or Linux
- **.NET Runtime**: .NET 8.0 or higher
- **Disk Space**: 50 MB for application + space for font backups
- **World of Warcraft**: Retail, Classic, or Classic Era client

### First Launch

1. Launch WoW Font Manager
2. The application will automatically scan for WoW installations
3. Detected clients appear in the client selector dropdown
4. If no clients are detected, manually specify your WoW installation path

## Detecting WoW Clients

### Automatic Detection

On startup, the application searches common installation paths:

- **Windows**: `C:\Program Files (x86)\World of Warcraft\`
- **macOS**: `/Applications/World of Warcraft/`
- **Linux**: Wine/Proton directories

### Manual Client Selection

If your WoW installation is in a non-standard location:

1. Click "Add Custom Client" button
2. Browse to your World of Warcraft installation directory
3. Select the root folder (contains `_retail_`, `_classic_`, etc.)
4. Application validates and adds the client

### Client Information Display

For each detected client, you'll see:
- **Client Type**: Retail, Classic, or Classic Era
- **Locale**: Current language setting (enUS, zhCN, etc.)
- **Version**: Game version number
- **Status**: Validation indicator (✓ Valid / ✗ Invalid)

## Browsing Fonts

### Opening Font Directory

1. Click "Browse Fonts" button
2. Navigate to a directory containing font files
3. Application scans for supported formats:
   - `.ttf` (TrueType Font)
   - `.otf` (OpenType Font)
   - `.ttc` (TrueType Collection)
   - `.woff` / `.woff2` (Web Fonts)

### Font List View

The font browser displays:
- **Font Name**: Family name from font metadata
- **File Name**: Actual file name on disk
- **Format**: Font file format
- **Glyphs**: Number of characters in the font
- **File Size**: Size in KB/MB

### Search and Filter

- **Search Box**: Type to filter fonts by name
- **Sort Options**: 
  - Name (A-Z / Z-A)
  - File Size (Smallest / Largest)
  - Glyph Count (Fewest / Most)

### Recent Directories

The application remembers your last 10 browsed directories for quick access.

## Previewing Fonts

### Preview Panel

Click any font in the list to see a live preview:

**Preview Options:**
- **Sample Text**: Edit to preview custom text
- **Font Size**: Adjust from 8pt to 72pt
- **Script**: Select language/writing system:
  - Latin (English, European)
  - Chinese Simplified
  - Chinese Traditional
  - Japanese
  - Korean
  - Numbers
  - Symbols

### Default Sample Text

Each script has appropriate sample text:
- **Latin**: "The quick brown fox jumps over the lazy dog"
- **Chinese (Simplified)**: "魔兽世界字体管理器" (WoW Font Manager)
- **Chinese (Traditional)**: "魔獸世界字體管理器"
- **Korean**: "월드 오브 워크래프트" (World of Warcraft)
- **Japanese**: "ワールド・オブ・ウォークラフト"

### Font Metadata

View detailed font information:

**Basic Info:**
- Font Family & Subfamily
- Version & Copyright
- Designer & Foundry

**Technical Details:**
- Format (TTF/OTF/TTC)
- Glyph Count
- Units Per Em
- Ascent / Descent / Line Gap

**Character Coverage:**
- Supported Unicode ranges
- CJK coverage percentage
- Special character blocks

## Replacing Fonts

### Step-by-Step Replacement

**1. Select WoW Client**
- Choose target client from dropdown
- Verify locale is correct
- Ensure WoW is **completely closed**

**2. Select Source Font**
- Browse and preview fonts
- Select font that will replace WoW fonts
- Check compatibility indicator (✓ Compatible)

**3. Choose Categories**

Select which font categories to replace:

- **All UI Fonts**: Replaces all game fonts (recommended)
- **Main UI Only**: Menus, tooltips, quest text
- **Chat Only**: Chat window and combat log
- **Damage Numbers Only**: Floating combat text

**4. Review and Replace**
- Click "Replace Fonts" button
- Review summary dialog showing:
  - Number of files to be replaced
  - Categories selected
  - Backup location
- Click "Confirm" to proceed

**5. Replacement Progress**
- Progress bar shows current status
- Files being replaced are listed
- Automatic backup created before replacement

### Important Warnings

⚠️ **WoW Must Be Closed**
- Application checks if WoW is running
- Refuses to replace if game is active
- Close ALL WoW processes before replacing

⚠️ **Font Compatibility**
- Use TTF format fonts only
- WoW does not support OTF or variable fonts
- For CJK locales, ensure 10,000+ glyphs

⚠️ **Backup Created Automatically**
- Every replacement creates a timestamped backup
- Backups stored in: `[AppData]/WowFontManager/Backups/`
- Old backups auto-deleted (keeps last 5)

## Restoring Backups

### Viewing Backups

1. Select WoW client
2. Click "Manage Backups" button
3. List shows all available backups:
   - Date and time created
   - Source font used
   - Categories replaced

### Restoring from Backup

1. Select backup from list
2. Click "Restore" button
3. Confirm restoration
4. Application copies original fonts back
5. Restart WoW to see changes

### Backup Management

**Auto-Cleanup:**
- Keeps 5 most recent backups per client
- Older backups automatically deleted
- Change limit in Settings

**Manual Cleanup:**
- Delete specific backups
- Clear all backups for a client
- Free up disk space

**Backup Location:**
```
Windows: %LOCALAPPDATA%\WowFontManager\Backups\
macOS: ~/Library/Application Support/WowFontManager/Backups/
Linux: ~/.config/WowFontManager/Backups/
```

## Troubleshooting

### WoW Client Not Detected

**Problem**: Application doesn't find your WoW installation

**Solutions**:
1. Use "Add Custom Client" to manually specify path
2. Ensure directory contains `_retail_`, `_classic_`, or `_classic_era_`
3. Check folder permissions (must be readable)
4. Verify WoW is fully installed (not corrupted)

### Font Replacement Failed

**Problem**: Replacement operation fails

**Solutions**:
1. **Close WoW completely**: Check Task Manager/Activity Monitor
2. **Run as Administrator**: Right-click → Run as Administrator (Windows)
3. **Check permissions**: Ensure write access to `Fonts/` directory
4. **Verify font format**: Use TTF fonts only
5. **Free disk space**: Ensure 500MB+ available

### In-Game Font Issues

**Problem**: Fonts look wrong or cause crashes

**Solutions**:

**Blocky/Pixelated Fonts:**
- Font missing required glyphs
- Use font with broader character coverage
- For CJK: Use fonts with 10,000+ glyphs

**Missing Characters (�):**
- Font doesn't support game's locale
- For zhCN/zhTW: Use fonts with CJK Unified Ideographs
- For ruRU: Use fonts with Cyrillic support

**Game Crashes on Startup:**
- Corrupted font file
- Restore from backup immediately
- Verify source font works in other applications

**UI Text Overlapping:**
- Font metrics don't match original
- Try different font or restore backup
- Adjust UI scale in-game settings

### Backup Restoration Problems

**Problem**: Cannot restore backup

**Solutions**:
1. Ensure WoW is closed
2. Check backup directory exists and is accessible
3. Verify backup metadata file (backup.json) is intact
4. Try manual restoration by copying files from backup folder

### Performance Issues

**Problem**: Slow font scanning or previews

**Solutions**:
1. Reduce number of fonts in directory (< 1000)
2. Clear application cache: Settings → Clear Cache
3. Scan non-network drives only
4. Disable preview auto-refresh during browsing

### Cache Issues

**Problem**: Previews not updating or corrupted

**Solutions**:
1. Settings → Clear All Caches
2. Restart application
3. Re-scan font directories
4. Check disk space (cache requires space)

## Best Practices

### Choosing Fonts for WoW

**For English (enUS):**
- Any TTF font works
- Recommended: fonts with good readability at small sizes
- Avoid overly decorative fonts for UI text

**For Chinese (zhCN/zhTW):**
- Minimum 10,000 glyphs required
- Recommended: Noto Sans CJK, Source Han Sans
- File size: 5-15 MB typical
- Avoid fonts > 30 MB (performance impact)

**For Korean (koKR):**
- Must include Hangul Syllables (U+AC00-U+D7AF)
- Recommended: Noto Sans KR, Nanum Gothic
- Ensure Latin character support for UI elements

**For Russian (ruRU):**
- Must include Cyrillic characters
- Most Western fonts support Cyrillic
- Test preview before replacing

### Backup Strategy

1. **Always test first**: Try font in one category before replacing all
2. **Keep system fonts**: Don't use system-critical fonts as source
3. **Regular cleanup**: Delete old backups to save space
4. **External backup**: Copy important backups to external drive
5. **Document changes**: Note which font you used in each backup

### Performance Considerations

**Font File Size:**
- Small (< 1 MB): Fast loading, basic coverage
- Medium (1-10 MB): Good balance
- Large (10-30 MB): Full CJK coverage, slower loading
- Very Large (> 30 MB): Avoid unless necessary

**Game Impact:**
- Fonts loaded once at game startup
- Larger fonts = longer load time
- No performance impact during gameplay
- UI rendering speed unaffected by font size

## Keyboard Shortcuts

- `Ctrl+O`: Open font directory
- `Ctrl+F`: Focus search box
- `Ctrl+R`: Replace fonts (when ready)
- `Ctrl+B`: Manage backups
- `Ctrl+S`: Open settings
- `F5`: Refresh font list
- `Esc`: Close current dialog

## Settings

Access settings via toolbar or `Ctrl+S`:

**General:**
- Default preview size
- Auto-detect clients on startup
- Number of recent directories to remember

**Preview:**
- Enable anti-aliasing
- Enable subpixel rendering
- Show font metrics overlay

**Backups:**
- Maximum backups to keep per client (default: 5)
- Backup storage location

**Advanced:**
- Enable debug logging
- Cache size limits
- Font scan thread count

## FAQ

**Q: Will replacing fonts get me banned?**
A: No. Font replacement is a client-side visual modification. Blizzard allows UI customization.

**Q: Do I need to replace fonts every patch?**
A: No. Replaced fonts persist through patches unless Blizzard updates the default fonts.

**Q: Can I use different fonts for different categories?**
A: Not directly. The app replaces all files in a category with the same source font.

**Q: What happens if I delete a backup?**
A: You can't restore that specific backup, but game fonts remain unchanged.

**Q: Can I share my font setup with friends?**
A: Yes! Share the source font file and note which categories you replaced.

## Support

For issues, feature requests, or questions:
- GitHub Issues: [project repository]
- Documentation: `/docs/` directory
- API Reference: `docs/API_REFERENCE.md`
