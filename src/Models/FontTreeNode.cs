using System.Collections.ObjectModel;

namespace WowFontManager.Models;

/// <summary>
/// Represents a node in the font tree hierarchy (either a folder or a font)
/// </summary>
public class FontTreeNode
{
    /// <summary>
    /// Display name for the node (folder name or font display name)
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Type of node (folder or font)
    /// </summary>
    public FontTreeNodeType NodeType { get; set; }

    /// <summary>
    /// Font information (null for folder nodes)
    /// </summary>
    public FontInfo? FontInfo { get; set; }

    /// <summary>
    /// Folder path (for folder nodes)
    /// </summary>
    public string? FolderPath { get; set; }

    /// <summary>
    /// Child nodes (fonts for folder nodes, empty for font nodes)
    /// </summary>
    public ObservableCollection<FontTreeNode> Children { get; set; } = new();

    /// <summary>
    /// Whether this node can be expanded (true for folders, false for fonts)
    /// </summary>
    public bool IsExpandable => NodeType == FontTreeNodeType.Folder;

    /// <summary>
    /// Count of fonts in this folder (for folder nodes)
    /// </summary>
    public int FontCount => NodeType == FontTreeNodeType.Folder ? Children.Count : 0;

    /// <summary>
    /// Display text with font count for folder nodes
    /// </summary>
    public string DisplayText => NodeType == FontTreeNodeType.Folder 
        ? $"{Name} ({FontCount})"
        : Name;
}

/// <summary>
/// Type of tree node
/// </summary>
public enum FontTreeNodeType
{
    Folder,
    Font
}
