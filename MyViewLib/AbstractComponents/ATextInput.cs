using NuFlexiArch;
using System.Text.Json.Serialization.Metadata;
using System.Text.Json.Serialization;

namespace MyViewLib;

[JsonSerializable(typeof(TextInputStateDto))]
public partial class TextInputSerializerContext : JsonSerializerContext { }
public class TextInputStateDto : CompStateDto
{
    public string? LabelValue { get; set; }
    public string? InputValue { get; set; }
}

public abstract class ATextInput : AComponent
{
    public abstract void SetLabelValue(string? val);
    public abstract string? GetLabelValue();
    public abstract void SetInputValue(string? val);
    public abstract string? GetInputValue();

    public override bool SetState(CompStateDto tempDto)
    {
        TextInputStateDto sDto = (TextInputStateDto)tempDto;
        SetLabelValue(sDto.LabelValue);
        SetInputValue(sDto.InputValue);

        return true;
    }

    public override CompStateDto GetState()
    {
        TextInputStateDto sDto = new();
        sDto.LabelValue = GetLabelValue();
        sDto.InputValue = GetInputValue();

        return sDto;
    }

    public override JsonTypeInfo GetJsonTypeInfo()
    {
        return TextInputSerializerContext.Default.TextInputStateDto;
    }
}

