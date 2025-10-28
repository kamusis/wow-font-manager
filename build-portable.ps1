#!/usr/bin/env pwsh
# Build script for WoW Font Manager - Portable Release

param(
    [Parameter(Mandatory=$false)]
    [ValidateSet('win-x64', 'osx-x64', 'osx-arm64', 'linux-x64', 'all')]
    [string]$Platform = 'win-x64',
    
    [Parameter(Mandatory=$false)]
    [switch]$Clean
)

$ErrorActionPreference = "Stop"
$ProjectPath = Join-Path $PSScriptRoot "src\WowFontManager.csproj"
$OutputBase = Join-Path $PSScriptRoot "publish"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "WoW Font Manager - Portable Build" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Clean previous builds if requested
if ($Clean -and (Test-Path $OutputBase)) {
    Write-Host "Cleaning previous builds..." -ForegroundColor Yellow
    Remove-Item $OutputBase -Recurse -Force
    Write-Host "✓ Cleaned" -ForegroundColor Green
    Write-Host ""
}

function Build-Platform {
    param([string]$RuntimeId)
    
    $OutputPath = Join-Path $OutputBase $RuntimeId
    
    Write-Host "Building for $RuntimeId..." -ForegroundColor Yellow
    Write-Host "Output: $OutputPath" -ForegroundColor Gray
    Write-Host ""
    
    # Build command
    dotnet publish $ProjectPath `
        --configuration Release `
        --runtime $RuntimeId `
        --output $OutputPath `
        --self-contained true `
        /p:PublishSingleFile=true `
        /p:IncludeNativeLibrariesForSelfExtract=true `
        /p:DebugType=None `
        /p:DebugSymbols=false `
        /p:EnableCompressionInSingleFile=true
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✓ Build successful for $RuntimeId" -ForegroundColor Green
        
        # Show output file info
        $exeName = if ($RuntimeId -like "win-*") { "WowFontManager.exe" } else { "WowFontManager" }
        $exePath = Join-Path $OutputPath $exeName
        
        if (Test-Path $exePath) {
            $fileSize = (Get-Item $exePath).Length / 1MB
            Write-Host "  File: $exeName" -ForegroundColor Gray
            Write-Host "  Size: $([math]::Round($fileSize, 2)) MB" -ForegroundColor Gray
        }
        Write-Host ""
    } else {
        Write-Host "✗ Build failed for $RuntimeId" -ForegroundColor Red
        exit 1
    }
}

# Build based on platform selection
if ($Platform -eq 'all') {
    Write-Host "Building for all platforms..." -ForegroundColor Cyan
    Write-Host ""
    
    Build-Platform 'win-x64'
    Build-Platform 'osx-x64'
    Build-Platform 'osx-arm64'
    Build-Platform 'linux-x64'
} else {
    Build-Platform $Platform
}

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Build Complete!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Output location: $OutputBase" -ForegroundColor Yellow
Write-Host ""
Write-Host "Usage:" -ForegroundColor Cyan
Write-Host "  1. Copy the executable to any directory" -ForegroundColor Gray
Write-Host "  2. Create a 'fonts' folder and add your font files" -ForegroundColor Gray
Write-Host "  3. Run WowFontManager and enjoy!" -ForegroundColor Gray
Write-Host ""
