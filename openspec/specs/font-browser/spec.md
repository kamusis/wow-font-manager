# font-browser Specification

## Purpose
TBD - created by archiving change improve-ui-contrast. Update Purpose after archive.
## Requirements
### Requirement: UI Contrast Standards
The system SHALL ensure all text and background color combinations meet WCAG AA accessibility standards for contrast ratios.

#### Scenario: Toolbar contrast
- **WHEN** the toolbar is displayed
- **THEN** the background color provides sufficient contrast with text elements
- **AND** the contrast ratio is at least 4.5:1 for normal text

#### Scenario: Status bar contrast
- **WHEN** the status bar is displayed
- **THEN** all text elements (status message, folder count) have sufficient contrast against the background
- **AND** the contrast ratio is at least 4.5:1 for normal text

#### Scenario: Preview panel contrast
- **WHEN** the preview panel is displayed without a selected font
- **THEN** the placeholder text has sufficient contrast against the background
- **AND** the contrast ratio is at least 4.5:1 for normal text

#### Scenario: No light-on-light combinations
- **WHEN** any UI element is rendered
- **THEN** no light-on-light color combinations are used
- **AND** text is clearly distinguishable from its background

### Requirement: Font Category Action Panel
The system SHALL provide a third panel to the right of the font preview panel that displays font category action buttons for future font replacement functionality.

#### Scenario: Action panel layout
- **WHEN** the font browser view is displayed
- **THEN** a third panel appears to the right of the font preview panel
- **AND** the panel has a width of approximately 200 pixels
- **AND** the panel contains a title "Apply Font"

#### Scenario: Action button arrangement
- **WHEN** the action panel is displayed
- **THEN** four action buttons are shown vertically from top to bottom
- **AND** the buttons are labeled: "All UI Fonts", "Main UI Only", "Chat Only", "Damage Numbers Only"
- **AND** each button has consistent styling and spacing

### Requirement: Font Category Action Commands
The system SHALL provide command infrastructure for font category actions that will enable future font replacement implementation.

#### Scenario: Button command binding
- **WHEN** the action panel is rendered
- **THEN** each button is bound to an `ApplyFontCommand` with the corresponding FontCategory parameter
- **AND** the command infrastructure is ready for future replacement logic

#### Scenario: Button enabled state with selection
- **WHEN** a font is selected in the font tree
- **THEN** all four category action buttons are enabled
- **AND** the buttons are visually indicated as clickable

#### Scenario: Button disabled state without selection
- **WHEN** no font is selected in the font tree
- **THEN** all four category action buttons are disabled
- **AND** the buttons are visually indicated as non-clickable

#### Scenario: Placeholder command execution
- **WHEN** a category action button is clicked
- **THEN** the command executes without errors
- **AND** the command prepares for future font replacement implementation (placeholder behavior)

### Requirement: Font Category Service
The system SHALL provide a service that maps font files to their WoW usage categories based on FontMappings.json.

#### Scenario: Category lookup by filename
- **WHEN** a font file name is provided to the category service
- **THEN** the service returns the appropriate categories (MainUI, Chat, Damage, or combinations)
- **AND** the lookup is based on the font file name matching entries in FontMappings.json

#### Scenario: Multiple category membership
- **WHEN** a font file appears in multiple category arrays in FontMappings.json
- **THEN** the service returns all categories that include the font file
- **AND** the "All" category includes fonts from all specific categories

#### Scenario: Locale-aware category mapping
- **WHEN** fonts from different locale folders (enUS, zhCN, zhTW, etc.) are loaded
- **THEN** the category service uses the appropriate locale section from FontMappings.json
- **AND** category mapping works correctly for each locale's font set

### Requirement: Action Panel Accessibility
The system SHALL ensure the action panel meets WCAG AA accessibility standards.

#### Scenario: Button contrast compliance
- **WHEN** action buttons are displayed in enabled or disabled states
- **THEN** all text and background color combinations meet WCAG AA contrast ratio standards
- **AND** the contrast ratio is at least 4.5:1 for normal text

#### Scenario: Tooltip information
- **WHEN** a user hovers over a category action button
- **THEN** a tooltip appears explaining the button's purpose
- **AND** the tooltip describes which WoW font files will be affected by this category

