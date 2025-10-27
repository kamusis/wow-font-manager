## Why
Users need a clear interface to replace WoW fonts by category (All UI Fonts, Main UI, Chat, Damage Numbers). This change lays the groundwork for the font replacement feature by establishing the UI structure and category system that will later enable users to apply selected fonts to specific WoW font categories.

## What Changes
- Add a third panel to the right of the font preview panel
- Add four vertically-arranged action buttons: "All UI Fonts", "Main UI Only", "Chat Only", "Damage Numbers Only"
- Establish font category system and mapping infrastructure using FontMappings.json
- Update UI layout to accommodate three-column design
- Prepare button command infrastructure for future font replacement implementation

## Impact
- Affected specs: `font-browser`
- Affected code:
  - `src/Views/FontBrowserView.axaml` - Add third panel with category action buttons
  - `src/ViewModels/FontBrowserViewModel.cs` - Add button commands and category state
  - `src/Models/FontCategory.cs` - New enum for WoW font categories
  - `src/Services/IFontCategoryService.cs` - Interface for category mapping service
  - `src/Services/FontCategoryService.cs` - Service to map font files to WoW categories using FontMappings.json
