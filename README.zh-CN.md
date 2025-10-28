# WoW 字体管理器

一款专为《魔兽世界》玩家设计的字体管理工具，可轻松浏览、预览和替换游戏字体。

![Screenshot](https://s2.loli.net/2025/10/28/hIdXWDy4MJg8nzN.png)

## 功能特性

- **字体浏览与预览**：实时渲染字体文件，支持多种字体格式（TTF、OTF、TTC、WOFF、WOFF2）
- **分类替换**：按类别替换游戏字体（主界面、聊天、战斗伤害数字）
- **多语言支持**：自动识别并支持 enUS、zhCN、zhTW等多个游戏语言版本（不同语言版本的游戏字体文件名有所不同）
- **CJK 优化**：针对中日韩字体进行特别优化，确保亚洲语言客户端的正确渲染
- **安全备份**：替换前自动备份原始字体

## 技术架构

### 核心技术栈

- **UI 框架**：Avalonia UI 11.3.6（跨平台 XAML 框架）
- **渲染引擎**：SkiaSharp 2.88.9（高性能 2D 图形库）
- **运行时**：.NET 9.0
- **开发语言**：C#
- **MVVM 框架**：ReactiveUI + Fody

## 支持平台

- Windows 10/11 (x64)
- macOS 10.15+ (x64, ARM64)

## 构建项目

### 前置要求

- .NET 9.0 SDK 或更高版本

### 构建命令

```bash
# 还原依赖
dotnet restore src/WowFontManager.csproj

# 调试构建
dotnet build src/WowFontManager.csproj

# 发布构建
dotnet build src/WowFontManager.csproj --configuration Release

# 发布 Windows x64 独立版本
dotnet publish src/WowFontManager.csproj -c Release -r win-x64 --self-contained

# 发布 macOS ARM64 版本
dotnet publish src/WowFontManager.csproj -c Release -r osx-arm64 --self-contained
```

### 便携版构建

Windows 用户可以使用提供的脚本快速构建便携版：

```bash
# PowerShell
.\build-portable.ps1

# 批处理
.\build-portable.bat
```

## 许可证

[Apache License 2.0](https://www.apache.org/licenses/LICENSE-2.0)

---

如需查看英文文档，请参阅 [README.md](README.md)。
