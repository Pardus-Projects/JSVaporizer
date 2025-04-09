using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace NuFlexiArch;

[JsonSerializable(typeof(TextDisplayDataDto))]
public partial class TextDisplaySerializerContext : JsonSerializerContext { }
public class TextDisplayDataDto : CompDataDto
{
    public string? Text { get; set; }
}

public abstract class ATextDisplay : AComponent
{
    public abstract void SetText(string? text);
    public abstract string? GetText();

    public override bool UpdateState(CompDataDto tempDto)
    {
        if (tempDto is TextDisplayDataDto tDto)
        {
            SetText(tDto.Text);
            return true;
        }
        throw new ArgumentException("Invalid DTO type for ATextDisplay.");
    }

    public override CompDataDto GetState()
    {
        return new TextDisplayDataDto
        {
            Text = GetText()
        };
    }

    public override JsonTypeInfo GetJsonTypeInfo()
    {
        return TextDisplaySerializerContext.Default.TextDisplayDataDto;
    }
}

