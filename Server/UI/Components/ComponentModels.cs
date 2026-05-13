using Server.API.Enums;
using Server.API.Routes.ImageFile.GET;

namespace Server.UI.Components;

public record TranslationFieldModel
{
    public required string LanguageCode { get; set; }
    public required string Text { get; set; }
    public required string OldLanguageCode { get; set; }
}

public record ImageInspectionModel
{
    public required ImageDTO Image { get; set; }
    public required int Width { get; set; }
    public required int Height { get; set; }

    public string GetProportions()
    {
        return Image.Type.ToLower() switch
        {
            "normal" => "16:9",
            "banner" => "3:1",
            "square" => "1:1",
            "icon" => "1:1",
            _ => "Okänd"
        };
    }
}

public record PageMetaModel
{
    public required Guid Id { get; set; }
    public required Dictionary<string, string> Title { get; set; } = new()
    {
        ["sv"] = "Namnlös sida"
    };
    public required Dictionary<string, string> Keywords { get; set; } = new()
    {
        ["sv"] = string.Empty
    };
    public required Dictionary<string, string> Description { get; set; } = new()
    {
        ["sv"] = string.Empty
    };
    public required Dictionary<string, string> Slug { get; set; } = new()
    {
        ["sv"] = "namnlos-sida"
    };
    public required PageType Type { get; set; } = PageType.Page;
    public required bool IsPublished { get; set; } = false;
    public required DateTime SavedAt { get; set; } = DateTime.UtcNow;
    public required DateTime? PublishedAt { get; set; } = null;
    public required DateTime? UpdatedAt { get; set; } = null;
}

public record PageEditorModel
{
    public required Guid Id { get; set; }
    public required PageMetaModel Meta { get; set; }
    public required Dictionary<string, string> Content { get; set; }
}

public record TabModel
{
    public required string LanguageCode { get; set; }
    public required string OldLanguageCode { get; set; }
    public required string Content { get; set; }
    public required Guid EditorId { get; set; }
}

public record MenuTreeNode
{
    public string Id { get; init; } = string.Empty;
    public string MenuItemId { get; init; } = string.Empty;
    public string Label { get; init; } = string.Empty;
    public List<MenuTreeNode> Children { get; init; } = [];
}

public record MenuTreePayload
{
    public required string InstanceId { get; init; }
    public required List<MenuTreeNode> Tree { get; init; }
}

public record MenuEditorInitResult
{
    public required string InstanceId { get; init; }
}