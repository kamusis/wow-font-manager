# WoW Font Manager - API Reference

Complete API documentation for all services and interfaces.

## Table of Contents

1. [WoW Client Service](#wow-client-service)
2. [Font Discovery Service](#font-discovery-service)
3. [Font Metadata Service](#font-metadata-service)
4. [Rendering Service](#rendering-service)
5. [Cache Service](#cache-service)
6. [Font Replacement Service](#font-replacement-service)
7. [Configuration Service](#configuration-service)
8. [Models](#models)

---

## WoW Client Service

Detects and manages World of Warcraft client installations.

### Interface: `IWoWClientService`

```csharp
public interface IWoWClientService
{
    Task<List<WoWClientConfiguration>> DetectClientsAsync();
    Task<bool> ValidateClientPathAsync(string path);
    List<string>? GetFontMappingForLocale(string locale, ReplacementCategory category);
    string? GetClientLocale(string clientPath);
    bool IsWoWRunning(string clientPath);
}
```

### Methods

#### DetectClientsAsync()

Automatically detects all installed WoW clients on the system.

**Returns:** `Task<List<WoWClientConfiguration>>` - List of detected clients

**Example:**
```csharp
var clients = await wowClientService.DetectClientsAsync();
foreach (var client in clients)
{
    Console.WriteLine($"{client.ClientType} - {client.Locale}");
}
```

#### ValidateClientPathAsync(string path)

Validates if a given path is a valid WoW client installation.

**Parameters:**
- `path` - Full path to potential WoW installation

**Returns:** `Task<bool>` - True if valid

**Example:**
```csharp
var isValid = await wowClientService.ValidateClientPathAsync(@"C:\Program Files (x86)\World of Warcraft\_retail_");
```

#### GetFontMappingForLocale(string locale, ReplacementCategory category)

Gets the list of font files to replace for a given locale and category.

**Parameters:**
- `locale` - WoW locale code (enUS, zhCN, zhTW, koKR, ruRU)
- `category` - Font category (All, MainUI, Chat, Damage)

**Returns:** `List<string>?` - List of font filenames, or null if not found

**Example:**
```csharp
var fontFiles = wowClientService.GetFontMappingForLocale("zhCN", ReplacementCategory.MainUI);
// Returns: ["ARKai_T.ttf", "FRIZQT__.ttf", "ZYKai_T.ttf"]
```

#### GetClientLocale(string clientPath)

Reads the active locale from WoW's Config.wtf file.

**Parameters:**
- `clientPath` - Path to WoW client directory

**Returns:** `string?` - Locale code or null if not found

**Example:**
```csharp
var locale = wowClientService.GetClientLocale(@"C:\Program Files (x86)\World of Warcraft\_retail_");
// Returns: "enUS"
```

#### IsWoWRunning(string clientPath)

Checks if any WoW process is currently running for the specified client.

**Parameters:**
- `clientPath` - Path to WoW client directory

**Returns:** `bool` - True if WoW is running

**Example:**
```csharp
if (wowClientService.IsWoWRunning(client.InstallationPath))
{
    Console.WriteLine("Please close WoW before replacing fonts.");
}
```

---

## Font Discovery Service

Discovers and enumerates font files in directories.

### Interface: `IFontDiscoveryService`

```csharp
public interface IFontDiscoveryService
{
    IAsyncEnumerable<FontFileEntry> ScanDirectoryAsync(
        string directoryPath,
        bool recursive = true,
        IProgress<int>? progress = null,
        CancellationToken cancellationToken = default);
    
    Task<List<FontFileEntry>> GetFontsInDirectoryAsync(string directoryPath);
}
```

### Methods

#### ScanDirectoryAsync()

Scans a directory for font files with streaming results.

**Parameters:**
- `directoryPath` - Directory to scan
- `recursive` - Include subdirectories (default: true)
- `progress` - Progress reporter (0-100)
- `cancellationToken` - Cancellation token

**Returns:** `IAsyncEnumerable<FontFileEntry>` - Stream of font entries

**Example:**
```csharp
await foreach (var font in fontDiscoveryService.ScanDirectoryAsync(@"C:\Fonts", true, progress))
{
    Console.WriteLine($"Found: {font.FontFamily}");
}
```

#### GetFontsInDirectoryAsync()

Gets all fonts in a directory (non-streaming).

**Parameters:**
- `directoryPath` - Directory to scan

**Returns:** `Task<List<FontFileEntry>>` - Complete list of fonts

**Example:**
```csharp
var fonts = await fontDiscoveryService.GetFontsInDirectoryAsync(@"C:\Fonts");
Console.WriteLine($"Found {fonts.Count} fonts");
```

---

## Font Metadata Service

Extracts detailed metadata from font files using SkiaSharp.

### Interface: `IFontMetadataService`

```csharp
public interface IFontMetadataService
{
    Task<FontMetadata?> LoadFontAsync(string filePath);
    Task<List<UnicodeRange>> DetectCoverageAsync(string filePath);
    Task<bool> ValidateFontAsync(string filePath);
}
```

### Methods

#### LoadFontAsync(string filePath)

Loads and extracts complete metadata from a font file.

**Parameters:**
- `filePath` - Path to font file

**Returns:** `Task<FontMetadata?>` - Font metadata or null if invalid

**Example:**
```csharp
var metadata = await fontMetadataService.LoadFontAsync(@"C:\Fonts\MyFont.ttf");
Console.WriteLine($"Font: {metadata.FontFamily}");
Console.WriteLine($"Glyphs: {metadata.GlyphCount}");
```

#### DetectCoverageAsync(string filePath)

Analyzes Unicode character coverage in the font.

**Parameters:**
- `filePath` - Path to font file

**Returns:** `Task<List<UnicodeRange>>` - List of supported Unicode ranges

**Example:**
```csharp
var ranges = await fontMetadataService.DetectCoverageAsync(@"C:\Fonts\MyFont.ttf");
foreach (var range in ranges)
{
    Console.WriteLine($"{range.Name}: {range.StartCode:X4}-{range.EndCode:X4}");
}
```

#### ValidateFontAsync(string filePath)

Validates if a font is compatible with WoW (TTF format, valid structure).

**Parameters:**
- `filePath` - Path to font file

**Returns:** `Task<bool>` - True if valid for WoW

**Example:**
```csharp
if (await fontMetadataService.ValidateFontAsync(fontPath))
{
    Console.WriteLine("Font is compatible with WoW");
}
```

---

## Rendering Service

Renders font previews with SkiaSharp.

### Interface: `IRenderingService`

```csharp
public interface IRenderingService
{
    Task<SKBitmap?> RenderPreviewAsync(PreviewConfiguration config, CancellationToken cancellationToken = default);
    SKRect MeasureText(string text, SKTypeface typeface, float fontSize);
    void RenderTextLayout(SKCanvas canvas, string text, SKTypeface typeface, float fontSize, SKPoint position, SKPaint paint);
    string GetDefaultSampleText(string locale);
}
```

### Methods

#### RenderPreviewAsync()

Renders a font preview image.

**Parameters:**
- `config` - Preview configuration (font, text, size, colors)
- `cancellationToken` - Cancellation token

**Returns:** `Task<SKBitmap?>` - Rendered preview bitmap

**Example:**
```csharp
var config = new PreviewConfiguration
{
    FilePath = @"C:\Fonts\MyFont.ttf",
    SampleText = "Hello World",
    FontSize = 24,
    Width = 800,
    Height = 200
};
var bitmap = await renderingService.RenderPreviewAsync(config);
```

#### MeasureText()

Measures the dimensions of rendered text.

**Parameters:**
- `text` - Text to measure
- `typeface` - Font typeface
- `fontSize` - Font size in points

**Returns:** `SKRect` - Text bounds

#### GetDefaultSampleText(string locale)

Gets appropriate sample text for a locale.

**Parameters:**
- `locale` - Locale code (enUS, zhCN, etc.)

**Returns:** `string` - Sample text

**Example:**
```csharp
var sampleText = renderingService.GetDefaultSampleText("zhCN");
// Returns: "快速的棕色狐狸跳过懒狗。\n魔兽世界字体管理器\n简体中文测试文本"
```

---

## Cache Service

Manages in-memory and disk caches for performance optimization.

### Interface: `ICacheService`

```csharp
public interface ICacheService : IDisposable
{
    SKTypeface GetOrAddTypeface(string filePath, Func<SKTypeface> factory);
    Task<SKBitmap?> GetOrAddThumbnailAsync(string key, Func<Task<SKBitmap?>> factory);
    Task<FontMetadata?> GetOrAddMetadataAsync(string filePath, Func<Task<FontMetadata?>> factory);
    void InvalidateFile(string filePath);
    void ClearAll();
    CacheStatistics GetStatistics();
}
```

### Methods

#### GetOrAddTypeface()

Gets a cached typeface or creates it if not cached.

**Parameters:**
- `filePath` - Font file path
- `factory` - Function to create typeface if not cached

**Returns:** `SKTypeface` - Cached or new typeface

**Example:**
```csharp
var typeface = cacheService.GetOrAddTypeface(fontPath, () => SKTypeface.FromFile(fontPath));
```

#### GetOrAddThumbnailAsync()

Gets a cached thumbnail or generates it.

**Parameters:**
- `key` - Cache key
- `factory` - Async function to create bitmap

**Returns:** `Task<SKBitmap?>` - Cached or new bitmap

#### GetStatistics()

Gets cache performance statistics.

**Returns:** `CacheStatistics` - Hit/miss rates and cache sizes

**Example:**
```csharp
var stats = cacheService.GetStatistics();
Console.WriteLine($"Typeface hits: {stats.TypefaceHits}, misses: {stats.TypefaceMisses}");
```

---

## Font Replacement Service

Handles font replacement with backup and restore capabilities.

### Interface: `IFontReplacementService`

```csharp
public interface IFontReplacementService
{
    Task<FontReplacementResult> ReplaceFontAsync(
        FontReplacementOperation operation,
        IProgress<int>? progress = null,
        CancellationToken cancellationToken = default);
    
    Task<RestoreResult> RestoreBackupAsync(
        WoWClientConfiguration client,
        BackupInfo backupInfo,
        IProgress<int>? progress = null,
        CancellationToken cancellationToken = default);
    
    Task<List<BackupInfo>> ListBackupsAsync(WoWClientConfiguration client);
    Task<int> CleanupOldBackupsAsync(WoWClientConfiguration client, int keepCount = 5);
    Task<ValidationResult> ValidateOperationAsync(FontReplacementOperation operation);
}
```

### Methods

#### ReplaceFontAsync()

Replaces WoW fonts with a custom font.

**Parameters:**
- `operation` - Replacement operation details
- `progress` - Progress reporter (0-100)
- `cancellationToken` - Cancellation token

**Returns:** `Task<FontReplacementResult>` - Operation result

**Example:**
```csharp
var operation = new FontReplacementOperation
{
    SourceFontPath = @"C:\MyFont.ttf",
    TargetClient = selectedClient,
    Categories = new List<ReplacementCategory> { ReplacementCategory.MainUI }
};
var result = await fontReplacementService.ReplaceFontAsync(operation, progress);
if (result.Success)
{
    Console.WriteLine($"Replaced {result.ReplacedFiles.Count} files");
}
```

#### RestoreBackupAsync()

Restores fonts from a backup.

**Parameters:**
- `client` - WoW client configuration
- `backupInfo` - Backup to restore
- `progress` - Progress reporter
- `cancellationToken` - Cancellation token

**Returns:** `Task<RestoreResult>` - Restore result

#### ListBackupsAsync()

Lists all available backups for a client.

**Parameters:**
- `client` - WoW client configuration

**Returns:** `Task<List<BackupInfo>>` - List of backups

**Example:**
```csharp
var backups = await fontReplacementService.ListBackupsAsync(client);
foreach (var backup in backups)
{
    Console.WriteLine($"{backup.BackupDate}: {backup.DisplayName}");
}
```

#### CleanupOldBackupsAsync()

Deletes old backups, keeping only the most recent.

**Parameters:**
- `client` - WoW client configuration
- `keepCount` - Number of backups to keep (default: 5)

**Returns:** `Task<int>` - Number of backups deleted

#### ValidateOperationAsync()

Validates a replacement operation before execution.

**Parameters:**
- `operation` - Operation to validate

**Returns:** `Task<ValidationResult>` - Validation result with errors/warnings

**Example:**
```csharp
var validation = await fontReplacementService.ValidateOperationAsync(operation);
if (!validation.IsValid)
{
    foreach (var error in validation.Errors)
    {
        Console.WriteLine($"Error: {error}");
    }
}
```

---

## Configuration Service

Manages application settings with automatic persistence.

### Interface: `IConfigurationService`

```csharp
public interface IConfigurationService
{
    Task<AppSettings> LoadSettingsAsync();
    Task SaveSettingsAsync(AppSettings settings);
    AppSettings GetSettings();
    Task UpdateSettingAsync<T>(string key, T value);
}
```

### Methods

#### LoadSettingsAsync()

Loads settings from storage.

**Returns:** `Task<AppSettings>` - Loaded settings

**Example:**
```csharp
var settings = await configService.LoadSettingsAsync();
Console.WriteLine($"Last directory: {settings.LastDirectory}");
```

#### SaveSettingsAsync()

Saves settings to storage.

**Parameters:**
- `settings` - Settings to save

**Example:**
```csharp
settings.PreviewSize = 32.0f;
await configService.SaveSettingsAsync(settings);
```

#### UpdateSettingAsync()

Updates a specific setting by property name.

**Parameters:**
- `key` - Property name
- `value` - New value

**Example:**
```csharp
await configService.UpdateSettingAsync("PreviewSize", 32.0f);
```

---

## Models

### WoWClientConfiguration

Represents a WoW client installation.

```csharp
public class WoWClientConfiguration
{
    public WoWClientType ClientType { get; set; }      // Retail, Classic, ClassicEra
    public string InstallationPath { get; set; }
    public string FontsPath { get; set; }
    public string Locale { get; set; }
    public bool IsValid { get; set; }
    public string? Version { get; set; }
    public string DisplayName { get; }                 // "WoW Retail (enUS)"
    public bool HasBackups { get; set; }
    public DateTime? LastBackupDate { get; set; }
}
```

### FontFileEntry

Discovered font file with basic metadata.

```csharp
public class FontFileEntry
{
    public string FilePath { get; set; }
    public string FileName { get; set; }
    public long FileSize { get; set; }
    public DateTime LastModified { get; set; }
    public string? FontFamily { get; set; }
    public string? FontSubfamily { get; set; }
    public FontFormat Format { get; set; }
    public bool IsValid { get; set; }
    public int GlyphCount { get; set; }
    public List<UnicodeRange> CoverageRanges { get; set; }
    public bool IsCompatibleWithWoW { get; set; }
}
```

### FontMetadata

Complete font metadata from SkiaSharp.

```csharp
public class FontMetadata
{
    public string FilePath { get; set; }
    public string FullName { get; set; }
    public string FontFamily { get; set; }
    public string FontSubfamily { get; set; }
    public FontFormat Format { get; set; }
    public int UnitsPerEm { get; set; }
    public int Ascent { get; set; }
    public int Descent { get; set; }
    public int LineGap { get; set; }
    public int GlyphCount { get; set; }
    public long FileSize { get; set; }
    public DateTime LastModified { get; set; }
}
```

### PreviewConfiguration

Configuration for rendering font previews.

```csharp
public class PreviewConfiguration
{
    public string? FilePath { get; set; }
    public string SampleText { get; set; }
    public float FontSize { get; set; } = 24.0f;
    public Script Script { get; set; } = Script.Latin;
    public bool ShowMetrics { get; set; } = false;
    public bool AntiAliasing { get; set; } = true;
    public bool SubpixelRendering { get; set; } = true;
    public SKColor BackgroundColor { get; set; } = SKColors.White;
    public SKColor TextColor { get; set; } = SKColors.Black;
    public int Width { get; set; } = 800;
    public int Height { get; set; } = 200;
}
```

### FontReplacementOperation

Represents a font replacement operation.

```csharp
public class FontReplacementOperation
{
    public string SourceFontPath { get; set; }
    public WoWClientConfiguration TargetClient { get; set; }
    public List<ReplacementCategory> Categories { get; set; }
    public List<string> TargetFiles { get; set; }
    public string? BackupPath { get; set; }
    public OperationStatus Status { get; set; }
    public int ReplacedCount { get; set; }
    public List<string> ErrorMessages { get; set; }
}
```

### BackupInfo

Information about a font backup.

```csharp
public class BackupInfo
{
    public DateTime BackupDate { get; set; }
    public string SourceFont { get; set; }
    public List<string> ReplacedFiles { get; set; }
    public WoWClientType ClientType { get; set; }
    public string Locale { get; set; }
    public List<ReplacementCategory> Categories { get; set; }
    public string BackupDirectory { get; set; }
    public string DisplayName { get; }
}
```

### AppSettings

Application settings.

```csharp
public class AppSettings
{
    public string? LastDirectory { get; set; }
    public List<string> RecentDirectories { get; set; }
    public int MaxRecentDirectories { get; set; } = 10;
    public float PreviewSize { get; set; } = 24.0f;
    public int WindowWidth { get; set; } = 1200;
    public int WindowHeight { get; set; } = 800;
    public bool WindowMaximized { get; set; } = false;
    public string Theme { get; set; } = "Light";
    public string? LastWoWClientPath { get; set; }
    public bool AutoDetectClients { get; set; } = true;
    public bool ShowPreviewMetrics { get; set; } = false;
    public bool EnableAntiAliasing { get; set; } = true;
    public bool EnableSubpixelRendering { get; set; } = true;
}
```

### Enums

```csharp
public enum WoWClientType
{
    Retail,
    Classic,
    ClassicEra
}

public enum ReplacementCategory
{
    All,
    MainUI,
    Chat,
    Damage
}

public enum FontFormat
{
    Unknown,
    TrueType,
    OpenType,
    TrueTypeCollection,
    WebFont
}

public enum OperationStatus
{
    Pending,
    InProgress,
    Completed,
    Failed,
    PartiallyCompleted
}

public enum Script
{
    Latin,
    ChineseSimplified,
    ChineseTraditional,
    Japanese,
    Korean,
    Numbers,
    Symbols,
    Mixed
}
```

---

## Usage Examples

### Complete Font Replacement Workflow

```csharp
// 1. Detect WoW clients
var clients = await wowClientService.DetectClientsAsync();
var client = clients.First();

// 2. Scan for fonts
var fonts = new List<FontFileEntry>();
await foreach (var font in fontDiscoveryService.ScanDirectoryAsync(@"C:\Fonts"))
{
    fonts.Add(font);
}

// 3. Load metadata for selected font
var selectedFont = fonts.First();
var metadata = await fontMetadataService.LoadFontAsync(selectedFont.FilePath);

// 4. Render preview
var preview = await renderingService.RenderPreviewAsync(new PreviewConfiguration
{
    FilePath = selectedFont.FilePath,
    SampleText = renderingService.GetDefaultSampleText(client.Locale),
    FontSize = 24
});

// 5. Validate and replace
var operation = new FontReplacementOperation
{
    SourceFontPath = selectedFont.FilePath,
    TargetClient = client,
    Categories = new List<ReplacementCategory> { ReplacementCategory.All }
};

var validation = await fontReplacementService.ValidateOperationAsync(operation);
if (validation.IsValid)
{
    var result = await fontReplacementService.ReplaceFontAsync(operation);
    Console.WriteLine($"Success: {result.Success}");
}

// 6. List and manage backups
var backups = await fontReplacementService.ListBackupsAsync(client);
await fontReplacementService.CleanupOldBackupsAsync(client, keepCount: 5);
```

### Caching Example

```csharp
// Use cache service for performance
var typeface = cacheService.GetOrAddTypeface(fontPath, () => SKTypeface.FromFile(fontPath));

var metadata = await cacheService.GetOrAddMetadataAsync(fontPath, async () =>
{
    return await fontMetadataService.LoadFontAsync(fontPath);
});

// Check cache statistics
var stats = cacheService.GetStatistics();
Console.WriteLine($"Cache efficiency: {stats.TypefaceHits * 100.0 / (stats.TypefaceHits + stats.TypefaceMisses)}%");
```
