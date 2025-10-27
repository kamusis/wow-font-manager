# Project Context

## Purpose
WoW Font Manager is a specialized font management tool for World of Warcraft players. It enables users to browse, preview, and replace game fonts with ease across multiple WoW client versions (Retail, Classic, Classic Era) and localizations (enUS, zhCN, zhTW). The tool ensures proper CJK font rendering for Asian language clients.

## Tech Stack
- **UI Framework**: Avalonia UI 11.3.6 (cross-platform XAML-based)
- **Rendering Engine**: SkiaSharp 2.88.9 (high-performance 2D graphics)
- **Runtime**: .NET 8.0
- **Language**: C# with nullable reference types enabled
- **MVVM Framework**: ReactiveUI with Fody
- **Additional Libraries**: CommunityToolkit.Mvvm 8.2.1

## Project Conventions

### Code Style
- **Language**: All code, comments, and documentation must be in English
- **Naming**: PascalCase for classes, methods, properties; camelCase for local variables and parameters
- **Nullability**: Nullable reference types enabled; use `?` suffix for nullable types
- **MVVM Pattern**: Strict separation of concerns (Models, ViewModels, Views, Services)
- **Async/Await**: Use async methods for I/O operations; suffix with `Async`
- **Interfaces**: Prefix with `I` (e.g., `IFontDiscoveryService`)

### Architecture Patterns
- **MVVM**: Models contain data, ViewModels contain logic and state, Views contain UI
- **Service Layer**: Business logic encapsulated in service interfaces with concrete implementations
- **Dependency Injection**: Services injected via constructor (prepare for DI container)
- **Reactive Programming**: Use ReactiveUI for property change notifications and commands
- **Separation of Concerns**: 
  - Models: Pure data structures in `Models/`
  - Services: Business logic in `Services/` with interface contracts
  - ViewModels: UI state and commands in `ViewModels/`
  - Views: XAML UI in `Views/`

### Testing Strategy
- **Unit Tests**: Test service layer logic in isolation
- **Integration Tests**: Test service interactions and file system operations
- **UI Tests**: Test ViewModels and view logic
- **Performance Targets**: 
  - Binary size: 15-25 MB
  - Preview rendering: <100ms
  - Font discovery: <500ms for 100 fonts

### Git Workflow
- **Commit Messages**: Follow conventional commits format with prefixes (feat, fix, refactor, etc.)
- **Branching**: Feature branches for new capabilities, main branch for stable code
- **OpenSpec Integration**: Use OpenSpec workflow for feature proposals and changes

## Domain Context

### WoW Font System
- **Font Categories**: 
  - Main UI fonts (menus, tooltips, general UI)
  - Chat fonts (chat window text)
  - Damage fonts (floating combat text)
- **Client Types**: Retail (current expansion), Classic (WotLK era), Classic Era (Vanilla)
- **Locales**: enUS (English), zhCN (Simplified Chinese), zhTW (Traditional Chinese)
- **Font Locations**: `<WoW Install>/Fonts/` directory contains replaceable font files
- **Backup Strategy**: Always backup original fonts before replacement

### Font Technical Requirements
- **Formats**: TrueType (.ttf), OpenType (.otf)
- **CJK Support**: Fonts for Asian locales must include comprehensive CJK glyph coverage
- **Unicode Ranges**: Track character coverage for compatibility validation
- **Metadata**: Extract family name, subfamily, PostScript name, version, embedding rights

## Important Constraints
- **Cross-Platform**: Must work on Windows 10/11, macOS 10.15+, and Linux (x64)
- **File System Access**: Requires read/write permissions to WoW installation directory
- **Process Safety**: Should detect if WoW is running before font replacement
- **Backup Integrity**: Must maintain backup metadata for restore operations
- **Performance**: Font preview rendering must be responsive (<100ms)
- **Memory**: Efficient caching for large font collections (LRU cache strategy)

## External Dependencies
- **Avalonia UI**: Cross-platform UI framework (no WPF/WinForms)
- **SkiaSharp**: 2D graphics rendering for font preview
- **ReactiveUI**: Reactive MVVM framework
- **File System**: Direct file I/O for font discovery and replacement
- **No External APIs**: Fully offline application, no network dependencies
