# WoW Font Manager

A specialized font management tool designed for World of Warcraft players to browse, preview, and replace game fonts with ease.

## Features

- **Font Browsing & Preview**: Real-time rendering of font files, supporting multiple formats (TTF, OTF, TTC, WOFF, WOFF2)
- **Category-based Replacement**: Replace game fonts by category (Main UI, Chat, Combat Damage)
- **Multi-language Support**: Automatically detects and supports multiple game locales like enUS, zhCN, zhTW (different locales use different font file names)
- **CJK Optimization**: Special optimizations for Chinese, Japanese, and Korean fonts to ensure proper rendering
- **Safe Backup**: Automatically backs up original fonts before replacement

## Technology Stack

### Core Technologies

- **UI Framework**: Avalonia UI 11.3.6 (cross-platform XAML-based)
- **Rendering Engine**: SkiaSharp 2.88.9 (high-performance 2D graphics)
- **Runtime**: .NET 9.0
- **Language**: C#
- **MVVM Framework**: ReactiveUI with Fody

## Supported Platforms

- Windows 10/11 (x64)
- macOS 10.15+ (x64, ARM64)

## Building the Project

### Prerequisites

- .NET 9.0 SDK or later

### Build Commands

```bash
# Restore dependencies
dotnet restore src/WowFontManager.csproj

# Build (Debug)
dotnet build src/WowFontManager.csproj

# Build (Release)
dotnet build src/WowFontManager.csproj --configuration Release

# Publish self-contained for Windows x64
dotnet publish src/WowFontManager.csproj -c Release -r win-x64 --self-contained

# Publish for macOS ARM64
dotnet publish src/WowFontManager.csproj -c Release -r osx-arm64 --self-contained
```

### Portable Build

Windows users can quickly build a portable version using the provided scripts:

```bash
# PowerShell
.\build-portable.ps1

# Batch
.\build-portable.bat
```

## License

[Apache License 2.0](https://www.apache.org/licenses/LICENSE-2.0)

---

For Chinese documentation, please refer to [README.zh-CN.md](README.zh-CN.md).
