using System.Text.Json.Serialization.Metadata;
using System.Text.Json.Serialization;

namespace NuFlexiArch;

[JsonSerializable(typeof(TextInputStateDto))]
public partial class TextInputSerializerContext : JsonSerializerContext { }

public class TextInputStateDto : CompStateDto
{
    public string? Label { get; set; }
    public string? InputValue { get; set; }
}

public abstract class ATextInput : AComponent
{
    public abstract void SetLabel(string? val);
    public abstract string? GetLabel();
    public abstract void SetInputValue(string? val);
    public abstract string? GetInputValue();

    public override bool SetState(CompStateDto tempDto)
    {
        if (tempDto is TextInputStateDto sDto)
        {
            SetLabel(sDto.Label);
            SetInputValue(sDto.InputValue);
            return true;
        }
        throw new ArgumentException("Invalid DTO type for ATextInput.");
    }

    public override CompStateDto GetState()
    {
        var sDto = new TextInputStateDto
        {
            Label = GetLabel(),
            InputValue = GetInputValue()
        };
        return sDto;
    }

    public override JsonTypeInfo GetJsonTypeInfo()
    {
        return TextInputSerializerContext.Default.TextInputStateDto;
    }
}