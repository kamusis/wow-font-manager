# 应用程序图标配置说明

## 图标文件位置

```
src/Assets/avalonia-logo.ico
```

## 配置说明

### 1. 应用程序图标（Windows EXE）

在 `WowFontManager.csproj` 中配置：

```xml
<ApplicationIcon>Assets\avalonia-logo.ico</ApplicationIcon>
```

**效果**:
- ✅ Windows 资源管理器中的文件图标
- ✅ 任务栏图标
- ✅ Alt+Tab 切换窗口时的图标
- ✅ 开始菜单快捷方式图标

### 2. 窗口标题栏图标

在 `MainWindow.axaml` 中配置：

```xml
<Window Icon="/Assets/avalonia-logo.ico">
```

**效果**:
- ✅ 窗口左上角标题栏图标
- ✅ 任务栏预览图标

## 图标要求

### Windows (.ico)
- **推荐尺寸**: 包含多个尺寸的图标
  - 16x16 (小图标)
  - 32x32 (标准图标)
  - 48x48 (大图标)
  - 256x256 (高分辨率)
- **格式**: ICO 格式
- **位深度**: 32-bit (支持透明度)

### macOS (.icns)
如果需要 macOS 支持，可以添加：

```xml
<PropertyGroup Condition="'$(RuntimeIdentifier)' == 'osx-x64' OR '$(RuntimeIdentifier)' == 'osx-arm64'">
  <CFBundleIconFile>avalonia-logo.icns</CFBundleIconFile>
</PropertyGroup>
```

### Linux (.png)
Linux 通常使用 PNG 格式，可以在桌面文件中指定。

## 如何更换图标

### 方法 1: 替换现有文件
1. 准备新的 ICO 文件
2. 重命名为 `avalonia-logo.ico`
3. 替换 `src/Assets/avalonia-logo.ico`
4. 重新构建项目

### 方法 2: 使用新文件名
1. 将新图标文件放到 `src/Assets/` 目录
2. 修改 `WowFontManager.csproj`:
   ```xml
   <ApplicationIcon>Assets\your-icon-name.ico</ApplicationIcon>
   ```
3. 修改 `MainWindow.axaml`:
   ```xml
   <Window Icon="/Assets/your-icon-name.ico">
   ```
4. 重新构建项目

## 在线图标转换工具

- **ICO 转换**: https://convertio.co/zh/png-ico/
- **多尺寸 ICO**: https://www.favicon-generator.org/
- **ICNS 转换**: https://cloudconvert.com/png-to-icns

## 验证图标

### 开发时
运行应用程序，检查：
- 窗口标题栏左上角是否显示图标
- 任务栏是否显示图标

### 发布后
构建 Release 版本后，检查：
```cmd
build-portable.bat
```

在 `publish\win-x64\` 目录中：
- 右键点击 `WowFontManager.exe` → 属性 → 查看图标
- 运行程序，检查窗口和任务栏图标

## 注意事项

1. **Assets 目录**: 所有资源文件都通过 `<AvaloniaResource Include="Assets\**" />` 自动包含
2. **路径格式**: 
   - `.csproj` 中使用反斜杠: `Assets\avalonia-logo.ico`
   - XAML 中使用正斜杠: `/Assets/avalonia-logo.ico`
3. **嵌入式资源**: 图标会自动嵌入到 EXE 中，发布后不需要单独的图标文件
4. **缓存问题**: 如果更换图标后没有变化，尝试清理构建缓存：
   ```cmd
   dotnet clean
   dotnet build
   ```

## 当前配置状态

✅ **已配置**:
- Windows EXE 图标
- 窗口标题栏图标
- 任务栏图标

❌ **未配置**:
- macOS .icns 图标
- Linux 桌面文件图标

如需跨平台图标支持，请参考 Avalonia 官方文档：
https://docs.avaloniaui.net/docs/guides/implementation-guides/how-to-add-an-icon
