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
The system SHALL execute font replacement workflow when "All UI Fonts" button is clicked, including confirmation dialog, backup creation, and file replacement.

#### Scenario: All UI Fonts button click with selection
- **WHEN** a font is selected in the font tree
- **AND** a WoW client is configured
- **AND** the user clicks the "All UI Fonts" button
- **THEN** a confirmation dialog is displayed
- **AND** the dialog shows the source font name and path
- **AND** the dialog shows the target WoW client type and locale
- **AND** the dialog shows a list of all copy commands that will be executed

#### Scenario: User confirms font replacement
- **WHEN** the confirmation dialog is displayed
- **AND** the user clicks "Replace Fonts" button
- **THEN** the system validates the operation (WoW not running, write permissions)
- **AND** creates a backup of the existing Fonts folder
- **AND** copies the selected font to all target font files based on locale mapping
- **AND** displays a success message with backup location
- **AND** closes the confirmation dialog

#### Scenario: User cancels font replacement
- **WHEN** the confirmation dialog is displayed
- **AND** the user clicks "Cancel" button
- **THEN** no file operations are performed
- **AND** the confirmation dialog is closed
- **AND** the user returns to the font browser view

#### Scenario: Validation failure before replacement
- **WHEN** the user confirms font replacement
- **AND** validation fails (e.g., WoW is running, no write permission)
- **THEN** an error message is displayed explaining the failure reason
- **AND** no backup or file operations are performed
- **AND** the confirmation dialog is closed

#### Scenario: Partial replacement success
- **WHEN** font replacement is in progress
- **AND** some files are successfully replaced but others fail
- **THEN** the operation continues for remaining files
- **AND** a summary message shows successful and failed file counts
- **AND** the backup is preserved for restoration

#### Scenario: Button disabled without WoW client
- **WHEN** a font is selected in the font tree
- **AND** no WoW client is configured
- **THEN** all four category action buttons are disabled
- **AND** a tooltip indicates "Configure WoW client first"

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

### Requirement: Font Replacement Confirmation Dialog
The system SHALL display a confirmation dialog before executing font replacement operations.

#### Scenario: Dialog displays operation details
- **WHEN** the confirmation dialog is shown
- **THEN** the dialog title is "Confirm Font Replacement"
- **AND** the source font name is displayed
- **AND** the source font full path is displayed
- **AND** the target WoW client type and locale are displayed
- **AND** the backup location path is displayed

#### Scenario: Dialog displays command preview
- **WHEN** the confirmation dialog is shown
- **THEN** a scrollable list of copy commands is displayed
- **AND** each command shows the format: `copy "{source}" "{target}"`
- **AND** the list includes all font files that will be replaced based on locale mapping
- **AND** the command count matches the target file count for the locale

#### Scenario: Dialog action buttons
- **WHEN** the confirmation dialog is shown
- **THEN** a "Replace Fonts" primary button is displayed
- **AND** a "Cancel" secondary button is displayed
- **AND** both buttons are enabled and clickable
- **AND** the dialog is modal (blocks interaction with main window)

### Requirement: Command Preview Generation
The system SHALL generate accurate preview of file copy operations with full absolute paths based on locale font mappings.

#### Scenario: Command format with full paths
- **WHEN** generating command preview
- **THEN** each command uses the format: `copy "{SourcePath}" "{TargetPath}"`
- **AND** source path is the full absolute path: `{AppInstallDir}\fonts\{SelectedFontFileName}`
- **AND** target path is the full absolute path: `d:\Games\World of Warcraft\_retail_\Fonts\{TargetFileName}`

#### Scenario: Preview for enUS locale
- **WHEN** generating command preview for enUS locale with "All" category
- **AND** the application is installed at `d:\tools\wtm\`
- **AND** the user selected font is `MyFont.ttf`
- **THEN** the preview includes 4 copy commands with full paths:
  - `copy "d:\tools\wtm\fonts\MyFont.ttf" "d:\Games\World of Warcraft\_retail_\Fonts\FRIZQT__.ttf"`
  - `copy "d:\tools\wtm\fonts\MyFont.ttf" "d:\Games\World of Warcraft\_retail_\Fonts\ARIALN.ttf"`
  - `copy "d:\tools\wtm\fonts\MyFont.ttf" "d:\Games\World of Warcraft\_retail_\Fonts\MORPHEUS.ttf"`
  - `copy "d:\tools\wtm\fonts\MyFont.ttf" "d:\Games\World of Warcraft\_retail_\Fonts\SKURRI.ttf"`

#### Scenario: Preview for zhCN locale
- **WHEN** generating command preview for zhCN locale with "All" category
- **AND** the application is installed at `d:\tools\wtm\`
- **AND** the user selected font is `云黑粗圆.ttf`
- **THEN** the preview includes 8 copy commands with full paths:
  - `copy "d:\tools\wtm\fonts\云黑粗圆.ttf" "d:\Games\World of Warcraft\_retail_\Fonts\ARKai_T.ttf"`
  - `copy "d:\tools\wtm\fonts\云黑粗圆.ttf" "d:\Games\World of Warcraft\_retail_\Fonts\FRIZQT__.ttf"`
  - `copy "d:\tools\wtm\fonts\云黑粗圆.ttf" "d:\Games\World of Warcraft\_retail_\Fonts\ZYKai_T.ttf"`
  - `copy "d:\tools\wtm\fonts\云黑粗圆.ttf" "d:\Games\World of Warcraft\_retail_\Fonts\ARHei.ttf"`
  - `copy "d:\tools\wtm\fonts\云黑粗圆.ttf" "d:\Games\World of Warcraft\_retail_\Fonts\ARIALN.ttf"`
  - `copy "d:\tools\wtm\fonts\云黑粗圆.ttf" "d:\Games\World of Warcraft\_retail_\Fonts\ZYHei.ttf"`
  - `copy "d:\tools\wtm\fonts\云黑粗圆.ttf" "d:\Games\World of Warcraft\_retail_\Fonts\ARKai_C.ttf"`
  - `copy "d:\tools\wtm\fonts\云黑粗圆.ttf" "d:\Games\World of Warcraft\_retail_\Fonts\ZYKai_C.ttf"`

#### Scenario: Preview for zhTW locale
- **WHEN** generating command preview for zhTW locale with "All" category
- **AND** the application is installed at `d:\tools\wtm\`
- **AND** the user selected font is `MyFont.ttf`
- **THEN** the preview includes 8 copy commands with full paths for Traditional Chinese fonts
- **AND** each command follows the format: `copy "d:\tools\wtm\fonts\MyFont.ttf" "d:\Games\World of Warcraft\_retail_\Fonts\{TargetFileName}"`
- **AND** target file names are: bLEI00D.ttf, bHEI01B.ttf, bHEI00M.ttf, bKAI00M.ttf, arheiuhk_bd.ttf, ARIALN.ttf, ARKai_C.ttf, FRIZQT__.ttf

