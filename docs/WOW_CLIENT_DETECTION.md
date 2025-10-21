# WoW Client Detection and Font Mapping

This document explains how the WoW Font Manager detects World of Warcraft client installations and maps fonts for different locales.

## Client Detection Process

### Detection Paths

The application searches for WoW installations in platform-specific locations:

**Windows:**
- `C:\Program Files (x86)\World of Warcraft\`
- `C:\Program Files\World of Warcraft\`

**macOS:**
- `/Applications/World of Warcraft/`
- `~/Applications/World of Warcraft/`

**Linux (Wine/Proton):**
- `~/.wine/drive_c/Program Files (x86)/World of Warcraft/`
- `~/.local/share/Steam/steamapps/compatdata/[appid]/pfx/drive_c/Program Files (x86)/World of Warcraft/`

### Client Types

The application detects three WoW client types based on subdirectory structure:

1. **Retail** - `_retail_` directory
2. **Classic** - `_classic_` directory  
3. **Classic Era** - `_classic_era_` directory

### Validation

For each detected client, the application validates:
- Fonts directory exists: `[client_path]/Fonts/`
- Directory is readable and writable
- Basic WoW executable structure is present

## Locale Detection

### Reading Locale from Config

The application reads the active locale from:
```
[client_path]/WTF/Config.wtf
```

Looking for the line:
```
SET locale "enUS"
```

### Supported Locales

- **enUS** - English (United States)
- **zhCN** - Chinese (Simplified)
- **zhTW** - Chinese (Traditional)
- **koKR** - Korean
- **ruRU** - Russian

## Font Mapping

### Font Categories

Fonts are organized into four categories:

1. **All** - All UI fonts
2. **MainUI** - Main interface fonts (menus, tooltips, quest text)
3. **Chat** - Chat window fonts
4. **Damage** - Combat damage/healing number fonts

### Locale-to-Font Mapping

The application uses an embedded JSON resource (`FontMappings.json`) to map each locale and category to specific font files in the WoW `Fonts/` directory.

#### Example Mapping for Chinese Simplified (zhCN):

```json
{
  "zhCN": {
    "All": [
      "ARKai_T.ttf",
      "FRIZQT__.ttf",
      "ZYKai_T.ttf",
      "ARHei.ttf",
      "ARIALN.ttf",
      "ZYHei.ttf",
      "ARKai_C.ttf",
      "ZYKai_C.ttf"
    ],
    "MainUI": ["ARKai_T.ttf", "FRIZQT__.ttf", "ZYKai_T.ttf"],
    "Chat": ["ARHei.ttf", "ARIALN.ttf", "ZYHei.ttf"],
    "Damage": ["ARKai_C.ttf", "ZYKai_C.ttf"]
  }
}
```

### Font File Purposes

**English Locales (enUS, enGB):**
- `FRIZQT__.ttf` - Primary UI font (Friz Quadrata)
- `ARIALN.ttf` - Arial Narrow for specific UI elements
- `skurri.ttf` - Skurri font for tooltips and certain UI text
- `MORPHEUS.ttf` - Morpheus font for mail, quest titles

**CJK Locales (zhCN, zhTW, koKR):**
- `ARKai_T.ttf` / `ZYKai_T.ttf` - Kaiti fonts for traditional characters
- `ARHei.ttf` / `ZYHei.ttf` - Heiti (sans-serif) fonts
- `ARKai_C.ttf` / `ZYKai_C.ttf` - Compressed Kaiti for damage numbers
- `FRIZQT__.ttf` - Fallback for Latin characters

**Russian (ruRU):**
- `FRIZQT___CYR.ttf` - Cyrillic version of Friz Quadrata
- `MORPHEUS_CYR.ttf` - Cyrillic Morpheus font
- `ARIALN.ttf` - Arial Narrow with Cyrillic support

## Replacement Process

### Pre-Replacement Checks

Before replacing fonts, the application validates:

1. **WoW Not Running** - Checks if any WoW process is active
   - Windows: `Wow.exe`, `WowClassic.exe`
   - macOS: `World of Warcraft.app`
   - Linux: Checks Wine processes

2. **Write Permissions** - Verifies write access to `Fonts/` directory

3. **Source Font Valid** - Confirms source font file exists and is readable

### Backup Creation

Before any replacement:

1. Creates timestamped backup directory:
   ```
   [AppData]/WowFontManager/Backups/[ClientType]_[Locale]/FontBackup_[timestamp]/
   ```

2. Copies all existing font files to backup directory

3. Saves backup metadata JSON:
   ```json
   {
     "BackupDate": "2024-01-15T10:30:00",
     "SourceFont": "/path/to/custom/font.ttf",
     "ReplacedFiles": ["ARKai_T.ttf", "FRIZQT__.ttf"],
     "ClientType": "Retail",
     "Locale": "zhCN",
     "Categories": ["MainUI"]
   }
   ```

### Font Replacement

For each target file in the selected category:

1. Copies source font to target location
2. Overwrites existing WoW font file
3. Records success/failure for each file

### Restore Process

To restore from backup:

1. Lists available backups (sorted by date)
2. Copies all files from backup directory back to `Fonts/`
3. Overwrites current fonts with original versions

## CJK Font Considerations

### Character Coverage

For Chinese, Japanese, and Korean locales, the application validates that replacement fonts include:

- **Basic Latin** (U+0000-U+007F)
- **CJK Unified Ideographs** (U+4E00-U+9FFF)
- **Hangul Syllables** (U+AC00-U+D7AF) for Korean
- **Hiragana/Katakana** (U+3040-U+30FF) for Japanese

### Font Size Requirements

CJK fonts typically require:
- Minimum 10,000+ glyphs for basic coverage
- 20,000+ glyphs for complete CJK Unified Ideographs
- TTF format (WoW does not support OTF or variable fonts)

### Performance Impact

Large CJK fonts can impact:
- Game loading time (fonts loaded at startup)
- Memory usage (all glyphs kept in memory)
- UI rendering performance

Recommended: Use subset fonts with only needed characters.

## Troubleshooting

### Client Not Detected

- Verify WoW installation path manually
- Check directory permissions
- Ensure `_retail_`, `_classic_`, or `_classic_era_` subdirectories exist

### Font Replacement Failed

- Ensure WoW is completely closed
- Run application with administrator/elevated privileges
- Check disk space availability
- Verify source font is valid TTF format

### In-Game Font Issues

- Fonts appear blocky: Font missing required glyphs
- Missing characters (�): Font lacks character coverage
- Game crashes on startup: Corrupted font file - restore from backup

## API Reference

### IWoWClientService

```csharp
// Detect all WoW clients
Task<List<WoWClientConfiguration>> DetectClientsAsync()

// Validate specific path
Task<bool> ValidateClientPathAsync(string path)

// Get font files for locale/category
List<string>? GetFontMappingForLocale(string locale, ReplacementCategory category)

// Read active locale from Config.wtf
string? GetClientLocale(string clientPath)

// Check if WoW is running
bool IsWoWRunning(string clientPath)
```

### Configuration Model

```csharp
public class WoWClientConfiguration
{
    public WoWClientType ClientType { get; set; }      // Retail/Classic/ClassicEra
    public string InstallationPath { get; set; }        // Full path to client
    public string FontsPath { get; set; }              // Path to Fonts directory
    public string Locale { get; set; }                 // Active locale (enUS, zhCN, etc.)
    public bool IsValid { get; set; }                  // Validation status
    public string? Version { get; set; }               // WoW version number
}
```
