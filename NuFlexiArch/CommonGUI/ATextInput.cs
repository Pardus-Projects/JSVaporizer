using System.Text.Json.Serialization.Metadata;
using System.Text.Json.Serialization;

namespace NuFlexiArch;

[JsonSerializable(typeof(TextInputDataDto))]
public partial class TextInputSerializerContext : JsonSerializerContext { }

public class TextInputDataDto : CompDataDto
{
    public string? Label { get; set; }
    public string? InputValue { get; set; }
}

public abstract class ATextInput : AComponent
{
    protected abstract void SetLabel(string? val);
    protected abstract string? GetLabel();
    protected abstract void SetInputValue(string? val);
    protected abstract string? GetInputValue();

    public override bool UpdateState(CompDataDto tempDto)
    {
        if (tempDto is TextInputDataDto sDto)
        {
            SetLabel(sDto.Label);
            SetInputValue(sDto.InputValue);
            return true;
        }
        throw new ArgumentException("Invalid DTO type for ATextInput.");
    }

    public override CompDataDto GetState()
    {
        var sDto = new TextInputDataDto
        {
            Label = GetLabel(),
            InputValue = GetInputValue()
        };
        return sDto;
    }

    public override JsonTypeInfo GetJsonTypeInfo()
    {
        return TextInputSerializerContext.Default.TextInputDataDto;
    }
}