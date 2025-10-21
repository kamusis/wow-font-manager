# WoW Font Manager

A specialized font management tool designed for World of Warcraft players to browse, preview, and replace game fonts with ease.

## Overview

**WoW Font Manager** enables WoW players to:
- Browse and preview font files with real-time rendering
- Select WoW client installations (Retail, Classic, Classic Era)
- Replace game fonts by category (Main UI, Chat, Combat Damage)
- Support multiple WoW localizations (enUS, zhCN, zhTW)
- Ensure proper CJK font rendering for Asian language clients

## Technology Stack

- **UI Framework**: Avalonia UI 11.3.6 (cross-platform XAML-based)
- **Rendering Engine**: SkiaSharp 2.88.9 (high-performance 2D graphics)
- **Runtime**: .NET 8.0
- **Language**: C#
- **MVVM Framework**: ReactiveUI with Fody

## Target Platforms

- Windows 10/11 (x64)
- macOS 10.15+ (x64, ARM64)
- Linux (x64)

## Project Structure

```
wow-font-manager/
├── src/                          # Main application source
│   ├── Models/                   # Data models
│   │   ├── BackupInfo.cs         # Backup metadata
│   │   ├── FontFileEntry.cs      # Font file representation
│   │   ├── FontMetadata.cs       # Detailed font metadata
│   │   ├── FontReplacementOperation.cs  # Replacement operations
│   │   ├── PreviewConfiguration.cs      # Preview settings
│   │   ├── UnicodeRange.cs       # Character coverage ranges
│   │   └── WoWClientConfiguration.cs    # WoW client config
│   ├── Services/                 # Service layer (TBD)
│   ├── ViewModels/               # MVVM ViewModels (TBD)
│   ├── Views/                    # UI Views (TBD)
│   ├── Resources/                # Embedded resources (TBD)
│   └── WowFontManager.csproj     # Project file
├── fonts/                        # Sample fonts directory
└── README.md                     # This file
```

## Current Implementation Status

### ✅ Completed

#### 1. Project Setup & Infrastructure
- [x] .NET 8.0 Avalonia UI project structure created
- [x] NuGet dependencies configured:
  - Avalonia 11.3.6
  - SkiaSharp 2.88.9
  - ReactiveUI.Fody 19.5.41
  - CommunityToolkit.Mvvm 8.2.1
- [x] Cross-platform build configuration (win-x64, osx-x64, osx-arm64, linux-x64)
- [x] Project folder structure established

#### 2. Core Data Models
- [x] **FontFileEntry**: Represents discovered font files with metadata
  - File path, name, size, modification date
  - Font family, subfamily, format
  - Glyph count and WoW compatibility flag
  - Unicode coverage ranges
  
- [x] **WoWClientConfiguration**: WoW client installation details
  - Client type (Retail/Classic/ClassicEra)
  - Installation and fonts paths
  - Locale detection
  - Backup status tracking
  
- [x] **FontMetadata**: Detailed font properties
  - Identification (family, subfamily, PostScript name)
  - Technical details (format, version, embedding rights)
  - Metrics (UnitsPerEm, Ascent, Descent, LineGap)
  - Designer information
  
- [x] **FontReplacementOperation**: Font replacement workflow
  - Source font and target client
  - Selected categories (All/MainUI/Chat/Damage)
  - Operation status and error tracking
  
- [x] **UnicodeRange**: Character coverage analysis
  - Unicode block definitions
  - Coverage percentage calculation
  
- [x] **PreviewConfiguration**: Font preview settings
  - Sample text and font size
  - Script selection (Latin, CJK, etc.)
  - Rendering options (anti-aliasing, subpixel)
  
- [x] **BackupInfo**: Backup metadata
  - Backup date and source font
  - Replaced files list
  - Client type and locale

### 🔄 In Progress / Pending

#### Service Layer
- [ ] WoW Client Service (auto-detection, locale mapping)
- [ ] Font Discovery Service (recursive scanning)
- [ ] Font Metadata Service (SkiaSharp extraction)
- [ ] Rendering Service (preview generation)
- [ ] Cache Service (LRU caching for performance)
- [ ] Font Replacement Service (backup/replace/restore)
- [ ] Configuration Service (settings persistence)

#### ViewModels & Views
- [ ] MainViewModel
- [ ] DirectoryViewModel  
- [ ] FontPreviewViewModel
- [ ] MetadataViewModel
- [ ] FontReplacementViewModel
- [ ] MainWindow UI
- [ ] Control panels and views

#### Additional Features
- [ ] Error handling & validation
- [ ] Performance optimizations
- [ ] Accessibility support
- [ ] Unit & integration tests
- [ ] Deployment packages

## Building the Project

### Prerequisites

- .NET 8.0 SDK or later
- Linux: `libicu-dev` package

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

## Design Document

The complete design specification is available in the project documentation, covering:
- Detailed architecture diagrams
- WoW font replacement mapping (enUS, zhCN, zhTW)
- Performance targets (15-25 MB binary, <100ms preview rendering)
- CJK font optimization strategies
- UI/UX specifications

## Development Roadmap

1. **Phase 1: Core Services** (Current)
   - Implement service layer interfaces
   - WoW client detection and validation
   - Font discovery and metadata extraction

2. **Phase 2: UI Implementation**
   - Create ViewModels with ReactiveUI
   - Build Avalonia views and controls
   - Implement font preview with SkiaSharp

3. **Phase 3: Font Replacement**
   - Backup and restore functionality
   - Font file copying with validation
   - Process detection and safety checks

4. **Phase 4: Polish & Release**
   - Performance optimization
   - Accessibility features
   - Testing and bug fixes
   - Deployment packages

## License

*(License to be determined)*

## Contributing

*(Contributing guidelines to be added)*

---

**Status**: Early Development - Core data models implemented, service layer in progress.
