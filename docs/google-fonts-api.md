# Google Fonts API Documentation

## Overview

This document describes the Google Fonts Developer API and its data structure, used for fetching available fonts programmatically.

## API Endpoint

```
GET https://www.googleapis.com/webfonts/v1/webfonts?key=YOUR-API-KEY
```

## Statistics

- **Total Fonts Available**: 1,899 fonts
- **Last Updated**: 2025-10-29

### Fonts by Locale/Subset

| Locale | Subset | Font Count |
|--------|--------|------------|
| **enUS** | `latin` | 1,883 |
| **zhCN** | `chinese-simplified` | 10 |
| **zhTW** | `chinese-traditional` | 13 |
| **jaJP** | `japanese` | 66 |
| **koKR** | `korean` | 38 |

### Chinese Subsets Breakdown

Google Fonts provides 3 separate Chinese subsets with a total of 26 fonts:

| Chinese Subset | Font Count |
|----------------|------------|
| `chinese-simplified` | 10 |
| `chinese-traditional` | 13 |
| `chinese-hongkong` | 3 |
| **Total** | **26** |

**Note**: The `chinese-traditional` subset (13 fonts) is used for Traditional Chinese in Taiwan (zhTW). The `chinese-hongkong` subset (3 fonts) is specifically for Hong Kong/Macau Traditional Chinese variants.

## Root Level JSON Structure

```json
{
  "kind": "webfonts#webfontList",
  "items": [ ... ]
}
```

### Root Fields

| Field | Type | Description |
|-------|------|-------------|
| `kind` | String | Always "webfonts#webfontList" - identifies the response type |
| `items` | Array | Array of font family objects |

---

## Font Object Structure

Each font in the `items` array has the following structure:

```json
{
  "family": "AR One Sans",
  "variants": [
    "regular",
    "500",
    "600",
    "700"
  ],
  "subsets": [
    "latin",
    "latin-ext",
    "vietnamese"
  ],
  "version": "v6",
  "lastModified": "2025-09-16",
  "category": "sans-serif",
  "kind": "webfonts#webfont",
  "files": {
    "regular": "https://fonts.gstatic.com/s/aronesans/v6/TUZezwhrmbFp0Srr_tH6fv6RcUejHO_u7GF5aXfv-U2QzBLF6gslWn_9DW03no5mBF4.ttf",
    "500": "https://fonts.gstatic.com/s/aronesans/v6/TUZezwhrmbFp0Srr_tH6fv6RcUejHO_u7GF5aXfv-U2QzBLF6gslWk39DW03no5mBF4.ttf",
    "600": "https://fonts.gstatic.com/s/aronesans/v6/TUZezwhrmbFp0Srr_tH6fv6RcUejHO_u7GF5aXfv-U2QzBLF6gslWqH6DW03no5mBF4.ttf",
    "700": "https://fonts.gstatic.com/s/aronesans/v6/TUZezwhrmbFp0Srr_tH6fv6RcUejHO_u7GF5aXfv-U2QzBLF6gslWpj6DW03no5mBF4.ttf"
  },
  "menu": "https://fonts.gstatic.com/s/aronesans/v6/TUZezwhrmbFp0Srr_tH6fv6RcUejHO_u7GF5aXfv-U2QzBLF6gslWn_9PWw9mg.ttf"
}
```

### Font Object Fields

| Field | Type | Description |
|-------|------|-------------|
| `family` | String | The name of the font family (e.g., "AR One Sans") |
| `variants` | Array[String] | Available font variants/styles. Can be "regular", "italic", "bold", or numeric weights (100-900). Examples: "regular", "italic", "700", "700italic", "500" |
| `subsets` | Array[String] | Character sets supported by the font. Examples: "latin", "latin-ext", "cyrillic", "greek", "vietnamese", "chinese", "japanese", "korean", "arabic", "hebrew" |
| `version` | String | Version identifier for the font (e.g., "v6") |
| `lastModified` | String | ISO date when the font was last modified (YYYY-MM-DD format) |
| `category` | String | Font classification: "sans-serif", "serif", "monospace", "display", or "handwriting" |
| `kind` | String | Always "webfonts#webfont" - identifies this as a font object |
| `files` | Object | Map of variant names to download URLs (TTF format from fonts.gstatic.com) |
| `menu` | String | URL to a preview/menu font file for displaying the font name |

---

## Variant Types

### Style Variants
- `regular` - Standard/normal weight and style
- `italic` - Italic style, normal weight
- `bold` or numeric weight (700) - Bold weight, normal style
- `bolditalic` or `700italic` - Bold weight, italic style

### Weight Variants
Numeric weights representing CSS font-weight:
- `100` - Thin
- `200` - Extra Light
- `300` - Light
- `400` - Normal (same as "regular")
- `500` - Medium
- `600` - Semi Bold
- `700` - Bold
- `800` - Extra Bold
- `900` - Black

---

## Character Subsets

Common subsets available:
- **Latin Scripts**: `latin`, `latin-ext`
- **Cyrillic**: `cyrillic`, `cyrillic-ext`
- **Greek**: `greek`, `greek-ext`
- **CJK (Chinese, Japanese, Korean)**:
  - **Chinese**: `chinese-simplified`, `chinese-traditional`, `chinese-hongkong`
  - **Japanese**: `japanese`
  - **Korean**: `korean`
- **Other Scripts**: `vietnamese`, `arabic`, `hebrew`, `thai`, `devanagari`, `adlam`, `ethiopic`, `sinhala`, etc.

---

## Font Categories

- **sans-serif** - Without serifs (clean, modern look)
- **serif** - With serifs (traditional look)
- **monospace** - Fixed-width fonts (for code)
- **display** - Decorative/stylized fonts (headlines)
- **handwriting** - Fonts that look hand-written

---

## Example Queries

### Get all fonts
```bash
curl "https://www.googleapis.com/webfonts/v1/webfonts?key=YOUR-API-KEY" | jq '.'
```

### Count total fonts
```bash
curl -s "https://www.googleapis.com/webfonts/v1/webfonts?key=YOUR-API-KEY" | jq '.items | length'
```

### Get first font
```bash
curl -s "https://www.googleapis.com/webfonts/v1/webfonts?key=YOUR-API-KEY" | jq '.items[0]'
```

### Filter by category
```bash
curl -s "https://www.googleapis.com/webfonts/v1/webfonts?key=YOUR-API-KEY" | jq '.items[] | select(.category=="serif")'
```

### Filter by subset (e.g., Chinese)
```bash
curl -s "https://www.googleapis.com/webfonts/v1/webfonts?key=YOUR-API-KEY" | jq '.items[] | select(.subsets[] | select(. == "chinese-traditional"))'
```

### Filter by multiple Chinese subsets
```bash
# Get all Chinese fonts (simplified, traditional, and Hong Kong variants)
curl -s "https://www.googleapis.com/webfonts/v1/webfonts?key=YOUR-API-KEY" | jq '[.items[] | select(.subsets[] | select(test("chinese")))] | length'
```

### Count fonts by specific locale
```bash
# Count Traditional Chinese fonts
curl -s "https://www.googleapis.com/webfonts/v1/webfonts?key=YOUR-API-KEY" | jq '[.items[] | select(.subsets[] | select(. == "chinese-traditional"))] | length'

# Count Japanese fonts
curl -s "https://www.googleapis.com/webfonts/v1/webfonts?key=YOUR-API-KEY" | jq '[.items[] | select(.subsets[] | select(. == "japanese"))] | length'

# Count Korean fonts
curl -s "https://www.googleapis.com/webfonts/v1/webfonts?key=YOUR-API-KEY" | jq '[.items[] | select(.subsets[] | select(. == "korean"))] | length'
```

---

## Implementation Notes for WoW Font Manager

### Caching Strategy
- Cache the complete font list locally to avoid repeated API calls
- Store in JSON format in `%LOCALAPPDATA%\WowFontManager\google-fonts-cache.json`
- Add a `lastCacheUpdate` timestamp to track freshness
- Refresh cache on application startup or via manual refresh button

### Data Model
Create a C# model to map the API response:

```csharp
public class GoogleFontFamily
{
    public string Family { get; set; }
    public List<string> Variants { get; set; }
    public List<string> Subsets { get; set; }
    public string Version { get; set; }
    public string LastModified { get; set; }
    public string Category { get; set; }
    public Dictionary<string, string> Files { get; set; }  // variant -> URL
    public string Menu { get; set; }
}

// Locale to Subset mapping for WoW locales
private static readonly Dictionary<string, string> LocaleToSubset = new()
{
    { "enUS", "latin" },                          // 1,883 fonts
    { "zhCN", "chinese-simplified" },             // 10 fonts
    { "zhTW", "chinese-traditional" },            // 13 fonts
    { "jaJP", "japanese" },                       // 66 fonts
    { "koKR", "korean" }                          // 38 fonts
};

// Helper method to get fonts by WoW locale
public List<GoogleFontFamily> GetFontsByWoWLocale(string locale)
{
    if (!LocaleToSubset.TryGetValue(locale, out var subset))
        return new List<GoogleFontFamily>();
    
    return allFonts.Where(f => f.Subsets.Contains(subset)).ToList();
}
```

### Download Considerations
- All font files are served from `fonts.gstatic.com` (Google's CDN)
- Files are in TTF format
- Implement rate limiting to respect API quotas
- Consider implementing parallel downloads with a thread pool
- Validate downloaded files before using them

### Preview Generation
- Use the `menu` URL to fetch a lightweight preview font file
- Use the full font files for actual replacement in WoW directories
- Consider caching downloaded fonts locally to reduce bandwidth usage

---

## Security Notes

⚠️ **API Key Management**
- Never hardcode API keys in the application
- Store API key in encrypted configuration file or environment variable
- Consider using a backend server to proxy API requests
- Alternatively, allow users to provide their own API keys
- Implement HTTP Referrer restrictions on the API key for additional security

---

## API Rate Limits and Quotas

- Google Fonts API is free to use
- No specific documented rate limits for the metadata endpoint
- However, Google may enforce limits at their discretion
- Implement caching and retry logic with exponential backoff

---

## References

- [Google Fonts Developer API Documentation](https://developers.google.com/fonts/docs/developer_api)
- [Google Fonts Official Website](https://fonts.google.com/)
