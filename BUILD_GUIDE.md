# WoW Font Manager - Build & Run Guide

## Prerequisites

### Required Software
- **.NET 8.0 SDK** (not just the runtime!)
  - Download: https://dotnet.microsoft.com/download/dotnet/8.0
  - Verify installation: `dotnet --version` should show 8.0.x

### Platform-Specific Requirements

**Linux:**
```bash
# Install ICU library (required for globalization)
sudo apt-get update
sudo apt-get install -y libicu-dev
```

**Windows/macOS:**
- No additional dependencies required

## 🏗️ Building the Project

### Option 1: Quick Development Build

```bash
# Navigate to project root
cd /path/to/wow-font-manager

# Restore dependencies
dotnet restore src/WowFontManager.csproj

# Build in Debug mode
dotnet build src/WowFontManager.csproj --configuration Debug

# Build in Release mode (optimized)
dotnet build src/WowFontManager.csproj --configuration Release
```

### Option 2: Build and Run Directly

```bash
# Run in Debug mode (with hot reload)
dotnet run --project src/WowFontManager.csproj

# Run in Release mode (optimized)
dotnet run --project src/WowFontManager.csproj --configuration Release
```

## 🚀 Running the Application

### Method 1: Using dotnet run (Recommended for Testing)

```bash
# From project root
dotnet run --project src/WowFontManager.csproj --configuration Release
```

### Method 2: Run Built Executable

**After building:**
```bash
# Windows
src\bin\Release\net8.0\WowFontManager.exe

# macOS/Linux
./src/bin/Release/net8.0/WowFontManager
```

## 📦 Creating Distributable Executables

### Windows Executable (Self-Contained)

```bash
dotnet publish src/WowFontManager.csproj \
  -c Release \
  -r win-x64 \
  --self-contained \
  -p:PublishSingleFile=true \
  -o ./publish/win-x64
```

Output: `publish/win-x64/WowFontManager.exe` (~77 MB)

### macOS Executable (Intel)

```bash
dotnet publish src/WowFontManager.csproj \
  -c Release \
  -r osx-x64 \
  --self-contained \
  -p:PublishSingleFile=true \
  -o ./publish/osx-x64
```

Output: `publish/osx-x64/WowFontManager` (~76 MB)

### macOS Executable (Apple Silicon)

```bash
dotnet publish src/WowFontManager.csproj \
  -c Release \
  -r osx-arm64 \
  --self-contained \
  -p:PublishSingleFile=true \
  -o ./publish/osx-arm64
```

### Linux Executable

```bash
dotnet publish src/WowFontManager.csproj \
  -c Release \
  -r linux-x64 \
  --self-contained \
  -p:PublishSingleFile=true \
  -o ./publish/linux-x64
```

**Note:** Linux builds require a display server (X11/Wayland) to run the GUI.

## 🧪 Testing the Application

### 1. Quick Functionality Test

After launching the app:

1. **Test WoW Detection:**
   - Click "Detect Clients" button
   - Should find installed WoW clients (Retail/Classic/PTR)
   - If no clients found, ensure WoW is installed in standard locations

2. **Test Font Browser:**
   - Click "Browse Fonts" 
   - Navigate to a folder with .ttf or .otf fonts
   - Should list all compatible fonts

3. **Test Font Preview:**
   - Select a font from the list
   - Should see preview in center panel
   - Adjust size slider (8-72pt) to test rendering
   - Preview shows English and Chinese text

4. **Test Font Replacement:**
   - Select a WoW client
   - Select a font
   - Click "Replace Fonts"
   - Should create backup and replace game fonts
   - Check status bar for success/error messages

5. **Test Backup Management:**
   - After replacement, backup should appear in right panel
   - Click "Refresh Backups" to reload list

### 2. Development Testing

If you're developing/testing changes:

```bash
# Run with hot reload (auto-restart on code changes)
dotnet watch --project src/WowFontManager.csproj
```

### 3. Unit Testing (If Test Project Exists)

```bash
# Run all tests
dotnet test

# Run with detailed output
dotnet test --verbosity detailed
```

## 🐛 Troubleshooting

### Build Errors

**Error: "The SDK 'Microsoft.NET.Sdk' specified could not be found"**
- Solution: Install .NET 8.0 SDK (not just runtime)

**Error: "ICU library not found" (Linux)**
```bash
sudo apt-get install -y libicu-dev
```

**Error: "Project file does not exist"**
- Ensure you're running commands from project root directory
- Verify path: `src/WowFontManager.csproj`

### Runtime Errors

**Error: "XOpenDisplay failed" (Linux)**
- Cause: No display server available
- Solution: Run on a system with GUI or use X11 forwarding
- Alternative: Build for Windows/macOS instead

**Warning: "Possible null reference return"**
- These are non-critical warnings
- Application will run normally

**macOS: "App can't be opened"**
```bash
# Remove quarantine attribute
xattr -cr ./publish/osx-x64/WowFontManager

# Or right-click → Open → confirm
```

### Performance Issues

**Slow startup:**
- Use Release build instead of Debug
- First run may be slower due to JIT compilation

**Font preview lag:**
- Large font files may take time to render
- Preview caching improves subsequent previews

## 📁 Project Structure

```
wow-font-manager/
├── src/
│   ├── WowFontManager.csproj   # Main project file
│   ├── Program.cs               # Application entry point
│   ├── App.axaml                # Application resources
│   ├── Models/                  # Data models
│   ├── Services/                # Business logic
│   ├── ViewModels/              # MVVM view models
│   └── Views/                   # UI views (AXAML)
├── publish/                     # Published executables
│   ├── win-x64/
│   ├── osx-x64/
│   └── linux-x64/
└── BUILD_GUIDE.md              # This file
```

## 🔍 Key Technologies

- **.NET 8.0** - Cross-platform framework
- **Avalonia UI 11.3.6** - XAML-based UI framework
- **ReactiveUI** - MVVM framework
- **SkiaSharp 2.88.9** - Font rendering engine

## 📝 Development Workflow

1. **Make code changes** in `src/` directory
2. **Test immediately**: `dotnet run --project src/WowFontManager.csproj`
3. **Build for release**: `dotnet build -c Release`
4. **Create distributable**: `dotnet publish` (see commands above)

## 🎯 Quick Commands Cheat Sheet

```bash
# Restore packages
dotnet restore src/WowFontManager.csproj

# Build Debug
dotnet build src/WowFontManager.csproj

# Build Release
dotnet build src/WowFontManager.csproj -c Release

# Run (Debug)
dotnet run --project src/WowFontManager.csproj

# Run (Release)
dotnet run --project src/WowFontManager.csproj -c Release

# Run with auto-reload
dotnet watch --project src/WowFontManager.csproj

# Publish Windows exe
dotnet publish src/WowFontManager.csproj -c Release -r win-x64 --self-contained -p:PublishSingleFile=true -o ./publish/win-x64

# Publish macOS app
dotnet publish src/WowFontManager.csproj -c Release -r osx-x64 --self-contained -p:PublishSingleFile=true -o ./publish/osx-x64

# Clean build artifacts
dotnet clean src/WowFontManager.csproj
```

## ✅ Verification Checklist

Before distributing:

- [ ] Application launches without errors
- [ ] WoW client detection works
- [ ] Font browsing shows fonts
- [ ] Font preview renders correctly
- [ ] Font replacement creates backups
- [ ] Font replacement succeeds
- [ ] Backup restore works
- [ ] Status messages are clear
- [ ] No console errors during operation
- [ ] Published executable runs on target OS

## 📞 Need Help?

If you encounter issues:

1. Check error messages in console output
2. Verify .NET SDK version: `dotnet --version`
3. Ensure all dependencies are installed
4. Try cleaning and rebuilding: `dotnet clean && dotnet build`
5. Check file permissions on Linux/macOS

Happy building! 🚀
