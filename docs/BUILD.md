# 构建说明 / Build Instructions

## 快速开始 / Quick Start

### Windows 用户

#### 方法 1: 使用批处理脚本（推荐）
```cmd
# 构建 Windows x64 版本
build-portable.bat

# 构建所有平台
build-portable.bat all

# 清理后重新构建
build-portable.bat win-x64 clean
```

#### 方法 2: 使用 PowerShell 脚本
```powershell
# 构建 Windows x64 版本
.\build-portable.ps1

# 构建所有平台
.\build-portable.ps1 -Platform all

# 清理后重新构建
.\build-portable.ps1 -Platform win-x64 -Clean
```

#### 方法 3: 直接使用 dotnet 命令
```cmd
dotnet publish src\WowFontManager.csproj -c Release -r win-x64
```

### macOS/Linux 用户

```bash
# 构建 macOS ARM64 版本 (Apple Silicon)
./build-portable.ps1 -Platform osx-arm64

# 构建 macOS x64 版本 (Intel)
./build-portable.ps1 -Platform osx-x64

# 构建 Linux x64 版本
./build-portable.ps1 -Platform linux-x64

# 构建所有平台
./build-portable.ps1 -Platform all
```

## 输出位置

构建完成后，可执行文件位于：

```
publish/
├── win-x64/
│   └── WowFontManager.exe      (~80-120 MB)
├── osx-x64/
│   └── WowFontManager          (~80-120 MB)
├── osx-arm64/
│   └── WowFontManager          (~80-120 MB)
└── linux-x64/
    └── WowFontManager          (~80-120 MB)
```

## 发布配置说明

项目已配置为自动构建 portable 单文件版本，包含以下特性：

- ✅ **单文件发布**: 所有依赖打包到一个可执行文件
- ✅ **自包含**: 包含 .NET 9.0 运行时，无需用户安装
- ✅ **原生库嵌入**: SkiaSharp 等原生库自动提取
- ✅ **压缩**: 启用单文件压缩，减小体积
- ✅ **无调试符号**: Release 版本移除调试信息
- ✅ **嵌入式资源**: FontMappings.json 打包在 EXE 中

## 高级选项

### 启用裁剪（减小文件大小）

⚠️ **警告**: 裁剪可能导致 Avalonia UI 运行时错误，不推荐使用

```cmd
dotnet publish src\WowFontManager.csproj -c Release -r win-x64 ^
    /p:PublishTrimmed=true ^
    /p:TrimMode=link
```

### 构建特定配置

```cmd
# Debug 版本（多文件，包含调试符号）
dotnet publish src\WowFontManager.csproj -c Debug -r win-x64

# Release 版本（单文件，优化）
dotnet publish src\WowFontManager.csproj -c Release -r win-x64
```

## 系统要求

### 开发环境
- .NET 9.0 SDK 或更高版本
- Windows 10/11, macOS 10.15+, 或 Linux

### 运行环境
- **Windows**: Windows 10 1607+ (x64)
- **macOS**: macOS 10.15+ (x64 或 ARM64)
- **Linux**: 现代 Linux 发行版 (x64)，需要 `libicu` 库

## 故障排除

### 构建失败

1. **检查 .NET SDK 版本**:
   ```cmd
   dotnet --version
   ```
   确保版本 >= 9.0

2. **清理构建缓存**:
   ```cmd
   dotnet clean src\WowFontManager.csproj
   dotnet restore src\WowFontManager.csproj
   ```

3. **检查磁盘空间**: 确保有足够空间（至少 500 MB）

### 运行时错误

1. **Linux 缺少依赖**:
   ```bash
   # Ubuntu/Debian
   sudo apt-get install libicu-dev
   
   # Fedora/RHEL
   sudo dnf install libicu
   ```

2. **macOS 权限问题**:
   ```bash
   chmod +x WowFontManager
   xattr -d com.apple.quarantine WowFontManager
   ```

## 文件大小优化

| 配置 | 大小 | 说明 |
|------|------|------|
| 默认（推荐） | ~100 MB | 单文件 + 压缩 + 自包含 |
| 启用裁剪 | ~60 MB | 可能不稳定，不推荐 |
| 框架依赖 | ~5 MB | 需要用户安装 .NET 9.0 |

## 发布清单

发布前检查：

- [ ] 更新版本号（如有）
- [ ] 运行测试（如有）
- [ ] 清理构建 `build-portable.bat win-x64 clean`
- [ ] 构建 Release 版本
- [ ] 测试可执行文件
- [ ] 创建发布包（ZIP）
- [ ] 编写更新日志

## 许可证

*(待定)*
