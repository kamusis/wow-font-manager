# font-replacement Specification

## Purpose
TBD - created by archiving change implement-font-replacement-ui. Update Purpose after archive.
## Requirements
### Requirement: Locale Detection from Config File
The system SHALL detect the WoW client locale by reading the Config.wtf file.

#### Scenario: Read locale from Config.wtf
- **WHEN** detecting the WoW client locale
- **AND** the WoW installation path is `d:\Games\World of Warcraft\_retail_\`
- **THEN** the system reads the file at `{WoWInstallPath}\WTF\Config.wtf`
- **AND** searches for a line matching the pattern `SET textLocale "xxXX"`
- **AND** extracts the locale code (e.g., "enUS", "zhCN", "zhTW")
- **AND** uses this locale for font mapping

#### Scenario: Config.wtf file not found
- **WHEN** detecting the WoW client locale
- **AND** the Config.wtf file does not exist at `{WoWInstallPath}\WTF\Config.wtf`
- **THEN** the system defaults to "enUS" locale
- **AND** displays a warning: "Could not detect WoW locale, defaulting to enUS"
- **AND** allows the user to proceed with replacement

#### Scenario: Locale line not found in Config.wtf
- **WHEN** reading the Config.wtf file
- **AND** no line matching `SET textLocale "xxXX"` is found
- **THEN** the system defaults to "enUS" locale
- **AND** displays a warning: "Could not detect WoW locale, defaulting to enUS"

#### Scenario: Supported locale codes
- **WHEN** a locale is detected from Config.wtf
- **THEN** the system supports the following locale codes: enUS, zhCN, zhTW, koKR, ruRU
- **AND** if an unsupported locale is detected, defaults to enUS with a warning

### Requirement: Font Mapping Based on Locale
The system SHALL use FontMappings.json to determine which font files to replace based on the detected locale and selected category.

#### Scenario: Load font mappings from JSON
- **WHEN** the font replacement feature is initialized
- **THEN** the system loads `FontMappings.json` from embedded resources
- **AND** parses the JSON to create locale-to-font mappings
- **AND** validates that all required locales (enUS, zhCN, zhTW, koKR, ruRU) are present

#### Scenario: Get font files for locale and category
- **WHEN** determining which fonts to replace
- **AND** the detected locale is "zhCN"
- **AND** the selected category is "All"
- **THEN** the system retrieves the font file list from `FontMappings.json["zhCN"]["All"]`
- **AND** returns the list: ["ARKai_T.ttf", "FRIZQT__.ttf", "ZYKai_T.ttf", "ARHei.ttf", "ARIALN.ttf", "ZYHei.ttf", "ARKai_C.ttf", "ZYKai_C.ttf"]

#### Scenario: Font mapping for enUS locale
- **WHEN** the detected locale is "enUS"
- **AND** the selected category is "All"
- **THEN** the system retrieves the font file list for English locale
- **AND** returns the list: ["FRIZQT__.ttf", "ARIALN.ttf", "MORPHEUS.ttf", "SKURRI.ttf"]

#### Scenario: Font mapping for zhTW locale
- **WHEN** the detected locale is "zhTW"
- **AND** the selected category is "All"
- **THEN** the system retrieves the font file list for Traditional Chinese locale
- **AND** returns the list: ["bLEI00D.ttf", "bHEI01B.ttf", "bHEI00M.ttf", "bKAI00M.ttf", "arheiuhk_bd.ttf", "ARIALN.ttf", "ARKai_C.ttf", "FRIZQT__.ttf"]

#### Scenario: FontMappings.json missing or invalid
- **WHEN** attempting to load FontMappings.json
- **AND** the file is missing or cannot be parsed
- **THEN** the system displays an error: "Font mapping configuration is missing or invalid"
- **AND** prevents the font replacement operation from proceeding

### Requirement: Fixed WoW Installation Path
The system SHALL use a hardcoded WoW installation path for this initial implementation.

#### Scenario: Fixed path configuration
- **WHEN** the font replacement feature is initialized
- **THEN** the WoW installation path is set to `d:\Games\World of Warcraft\_retail_\`
- **AND** the Fonts folder path is derived as `{WoWInstallPath}\Fonts`
- **AND** the Config.wtf path is derived as `{WoWInstallPath}\WTF\Config.wtf`
- **AND** these paths are used for all font replacement operations

#### Scenario: Fixed path validation
- **WHEN** the user attempts to replace fonts
- **THEN** the system validates that the Fonts folder exists at `{WoWInstallPath}\Fonts`
- **AND** if the Fonts folder does not exist, an error is shown: "WoW Fonts folder not found at: {WoWInstallPath}\Fonts"
- **AND** the font replacement operation is prevented

#### Scenario: Fixed path in confirmation dialog
- **WHEN** the confirmation dialog is displayed
- **THEN** the WoW installation path is shown as: "WoW Install: d:\Games\World of Warcraft\_retail_\"
- **AND** the target Fonts folder is shown as: "Target: {WoWInstallPath}\Fonts"
- **AND** the user can see exactly where fonts will be replaced

### Requirement: Backup via Folder Rename
The system SHALL create backups by renaming the existing Fonts folder with a timestamp suffix.

#### Scenario: Backup by renaming Fonts folder
- **WHEN** creating a backup before font replacement
- **AND** the WoW installation path is `d:\Games\World of Warcraft\_retail_\`
- **AND** the Fonts folder exists at `{WoWInstallPath}\Fonts`
- **THEN** the existing Fonts folder is renamed to `Fonts.{timestamp}`
- **AND** the timestamp format is `yyyyMMdd_HHmmss`
- **AND** the renamed folder is in the WoW installation directory: `{WoWInstallPath}\Fonts.{timestamp}`
- **AND** a new empty Fonts folder is created at `{WoWInstallPath}\Fonts`

#### Scenario: No existing Fonts folder
- **WHEN** creating a backup before font replacement
- **AND** the Fonts folder does not exist at `{WoWInstallPath}\Fonts`
- **THEN** no backup is created (nothing to backup)
- **AND** a new Fonts folder is created at `{WoWInstallPath}\Fonts`
- **AND** font replacement proceeds normally

#### Scenario: Backup folder name collision
- **WHEN** creating a backup
- **AND** a folder with the name `Fonts.{timestamp}` already exists (unlikely but possible)
- **THEN** the system appends a sequential number to the folder name
- **AND** the format becomes `Fonts.{timestamp}_2`, `Fonts.{timestamp}_3`, etc.
- **AND** the rename operation succeeds with the unique name

#### Scenario: Backup metadata preservation
- **WHEN** a backup is created by renaming
- **THEN** a `backup.json` file is saved in the renamed backup folder
- **AND** the metadata includes: backup date, source font path, target files list, client type, locale
- **AND** the metadata can be used for restoration operations

### Requirement: Pre-Replacement Validation
The system SHALL validate all preconditions before performing any file operations.

#### Scenario: WoW running check
- **WHEN** validating a font replacement operation
- **THEN** the system checks if any WoW process is running
- **AND** if WoW is running, validation fails with error: "World of Warcraft is currently running. Please close the game before replacing fonts."

#### Scenario: Write permission check
- **WHEN** validating a font replacement operation
- **THEN** the system attempts to create a test file in the Fonts directory
- **AND** if the test file creation fails, validation fails with error: "No write permission to WoW fonts directory"
- **AND** the test file is deleted if successfully created

#### Scenario: Source font existence check
- **WHEN** validating a font replacement operation
- **THEN** the system verifies the source font file exists
- **AND** if the source font does not exist, validation fails with error: "Source font file does not exist"

#### Scenario: Target directory existence check
- **WHEN** validating a font replacement operation
- **THEN** the system verifies the WoW Fonts directory exists
- **AND** if the directory does not exist, validation fails with error: "WoW client fonts directory does not exist"

### Requirement: Font File Replacement Process
The system SHALL replace font files with proper error handling and progress reporting.

#### Scenario: Sequential file replacement
- **WHEN** executing font replacement after successful validation
- **THEN** the system copies the source font to each target font file
- **AND** each copy operation overwrites the existing target file
- **AND** if a copy operation fails, the error is recorded but processing continues
- **AND** all successful and failed operations are tracked

#### Scenario: Backup creation before replacement
- **WHEN** executing font replacement
- **THEN** the backup is created before any font files are replaced
- **AND** if backup creation fails, the entire operation is aborted
- **AND** no font files are modified if backup fails

#### Scenario: Replacement result reporting
- **WHEN** font replacement completes
- **THEN** a result object is returned with success status
- **AND** the result includes list of successfully replaced files
- **AND** the result includes list of failed files with error messages
- **AND** the result includes backup information (location, date)

### Requirement: Error Handling and Recovery
The system SHALL handle errors gracefully and preserve data integrity.

#### Scenario: Complete success
- **WHEN** all font files are successfully replaced
- **THEN** the result success status is true
- **AND** the success message includes: "Successfully replaced {count} fonts. Backup created at: {path}"
- **AND** the failed files list is empty

#### Scenario: Partial success
- **WHEN** some font files are successfully replaced but others fail
- **THEN** the result success status is false
- **AND** the error message includes: "Replaced {success_count} of {total_count} fonts. Failed: {failed_list}. Backup at: {path}"
- **AND** the backup remains available for restoration

#### Scenario: Complete failure
- **WHEN** no font files are successfully replaced
- **THEN** the result success status is false
- **AND** the error message includes: "Font replacement failed: {reason}. No changes made."
- **AND** if backup was created, it can be used for restoration

#### Scenario: Backup rename failure
- **WHEN** folder rename fails (e.g., permission error, folder in use)
- **THEN** the operation is aborted immediately
- **AND** no font files are modified
- **AND** an error message is displayed: "Backup creation failed: {reason}. Font replacement cancelled."
- **AND** the original Fonts folder remains unchanged

### Requirement: Backup Restoration Support
The system SHALL support restoring fonts from backups created in WoW directory.

#### Scenario: List backups from WoW directory
- **WHEN** listing available backups for a WoW client
- **THEN** the system scans for folders matching `Fonts.{timestamp}` pattern
- **AND** folders are sorted by timestamp (newest first)
- **AND** each backup's metadata is loaded from `backup.json` if available

#### Scenario: Restore from WoW directory backup
- **WHEN** restoring a backup from WoW directory
- **THEN** all font files from the backup folder are copied to the Fonts directory
- **AND** existing files in Fonts directory are overwritten
- **AND** the restoration result includes success/failure status and file lists

