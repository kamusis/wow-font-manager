# 快速构建指南 / Quick Build Guide

## 🚀 一键构建 / One-Click Build

### Windows

```cmd
build-portable.bat
```

输出: `publish\win-x64\WowFontManager.exe`

---

## 📦 所有平台 / All Platforms

```cmd
build-portable.bat all
```

或

```powershell
.\build-portable.ps1 -Platform all
```

---

## 🧹 清理重建 / Clean Build

```cmd
build-portable.bat win-x64 clean
```

---

## 📋 构建选项 / Build Options

| 平台 | 命令 | 输出文件 |
|------|------|----------|
| Windows x64 | `build-portable.bat win-x64` | `WowFontManager.exe` |
| macOS Intel | `.\build-portable.ps1 -Platform osx-x64` | `WowFontManager` |
| macOS Apple Silicon | `.\build-portable.ps1 -Platform osx-arm64` | `WowFontManager` |
| Linux x64 | `.\build-portable.ps1 -Platform linux-x64` | `WowFontManager` |

---

## ✅ 构建完成后

1. 进入 `publish\win-x64\` 目录
2. 复制 `WowFontManager.exe` 到任意位置
3. 在同目录创建 `fonts` 文件夹
4. 添加字体文件到 `fonts` 目录
5. 运行 `WowFontManager.exe`

---

## 🔧 故障排除 / Troubleshooting

### 找不到 dotnet 命令

安装 .NET 9.0 SDK: https://dotnet.microsoft.com/download

### 构建失败

```cmd
dotnet clean src\WowFontManager.csproj
dotnet restore src\WowFontManager.csproj
build-portable.bat win-x64 clean
```

---

## 📖 详细文档

查看 `BUILD.md` 获取完整构建说明
