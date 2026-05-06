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