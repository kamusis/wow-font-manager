# macOS Local Packaging Test Guide

This guide explains how to build and test the macOS `.app` bundle locally for WowFontManager.

## Prerequisites
- macOS with Xcode command line tools (for `PlistBuddy`, `iconutil`, `sips` if needed).
- .NET SDK 9.x installed (`dotnet --version`).
- Project dependency added: Dotnet.Bundle (already in the project):
  ```bash
  dotnet add src/WowFontManager.csproj package Dotnet.Bundle --version 0.9.13
  ```
- App icon file present at:
  - `src/Assets/wowfm-osx-logo.icns`

## Build the .app Bundle
Choose the appropriate Runtime Identifier (RID):
- Apple Silicon: `osx-arm64`
- Intel: `osx-x64`

Run from the repo root:
```bash
RID=osx-arm64  # change to osx-x64 on Intel

dotnet msbuild src/WowFontManager.csproj \
  -t:BundleApp \
  -p:RuntimeIdentifier=$RID \
  -p:Configuration=Release \
  -p:UseAppHost=true \
  -p:PublishSingleFile=false \
  -p:CFBundleName=WowFontManager \
  -p:CFBundleDisplayName="WowFontManager" \
  -p:CFBundleIdentifier=me.kamusis.wowfontmanager \
  -p:CFBundleShortVersionString=1.0.0 \
  -p:CFBundleVersion=1.0.0
```

Locate the generated app:
```bash
APP_PATH=$(find src/bin/Release/net9.0/$RID/publish -maxdepth 1 -name "*.app" -print -quit)
echo "APP_PATH=$APP_PATH"
```

## Inject Fonts into the App
If you want the bundled fonts available inside the app:
```bash
if [ -d fonts ]; then
  mkdir -p "$APP_PATH/Contents/MacOS/fonts"
  rsync -a fonts/ "$APP_PATH/Contents/MacOS/fonts/"
fi
```

## Set the App Icon (.icns)
Copy the prebuilt `.icns` and set `CFBundleIconFile`:
```bash
cp src/Assets/wowfm-osx-logo.icns "$APP_PATH/Contents/Resources/wowfm-osx-logo.icns"
PLIST="$APP_PATH/Contents/Info.plist"
/usr/libexec/PlistBuddy -c "Set :CFBundleIconFile wowfm-osx-logo.icns" "$PLIST" 2>/dev/null || \
/usr/libexec/PlistBuddy -c "Add :CFBundleIconFile string wowfm-osx-logo.icns" "$PLIST"
```

## Gatekeeper (Quarantine) Workaround
If macOS shows "App is damaged and can’t be opened" after downloading from the web, remove the quarantine attribute:
```bash
xattr -dr com.apple.quarantine "$APP_PATH"
```
Then launch:
```bash
open -n "$APP_PATH"
```

## Optional: Create a Local .tar.gz
Stage the `.app` with a README and archive it (useful for manual distribution):
```bash
STAGE_DIR=macpkg-$RID
rm -rf "$STAGE_DIR" && mkdir -p "$STAGE_DIR"
cp -R "$APP_PATH" "$STAGE_DIR/"
cat > "$STAGE_DIR/README.txt" << 'EOF'
WoW Font Manager (macOS)
========================

Usage:
1) Extract this archive
2) Remove quarantine if necessary:
   xattr -dr com.apple.quarantine "WowFontManager.app"
3) Double-click WowFontManager.app to launch

Notes:
- The app bundle contains a fonts directory at Contents/MacOS/fonts
EOF

tar -C "$STAGE_DIR" -czf "WowFontManager-$RID.tar.gz" .
```

## Tips
- If Finder does not immediately show the new icon, close and reopen the folder or wait for the icon cache to refresh.
- To test Intel vs Apple Silicon outputs on the same machine, you can build both RIDs; running x64 on Apple Silicon requires Rosetta.
- If you change the display name (`CFBundleDisplayName`), the `.app` name will also change.
