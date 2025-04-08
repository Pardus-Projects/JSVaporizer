using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace NuFlexiArch;

[JsonSerializable(typeof(TextDisplayDto))]
public partial class TextDisplaySerializerContext : JsonSerializerContext { }
public class TextDisplayDto : CompStateDto
{
    public string? Text { get; set; }
}

public abstract class ATextDisplay : AComponent
{
    public abstract void SetText(string? text);
    public abstract string? GetText();

    public override bool SetState(CompStateDto tempDto)
    {
        if (tempDto is TextDisplayDto tDto)
        {
            SetText(tDto.Text);
            return true;
        }
        throw new ArgumentException("Invalid DTO type for ATextDisplay.");
    }

    public override CompStateDto GetState()
    {
        return new TextDisplayDto
        {
            Text = GetText()
        };
    }

    public override JsonTypeInfo GetJsonTypeInfo()
    {
        return TextDisplaySerializerContext.Default.TextDisplayDto;
    }
}

