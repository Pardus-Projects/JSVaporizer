using System.Text.Json.Serialization.Metadata;
using System.Text.Json.Serialization;

namespace NuFlexiArch;

[JsonSerializable(typeof(ButtonStateDto))]
public partial class ButtonSerializerContext : JsonSerializerContext { }

public class ButtonStateDto : CompStateDto
{
    public string? Label { get; set; }
    public bool IsDisabled { get; set; }
}

public abstract class AButton : AComponent
{
    public abstract void SetLabel(string? label);
    public abstract string? GetLabel();

    public abstract void SetDisabled(bool isDisabled);
    public abstract bool GetDisabled();

    public override bool SetState(CompStateDto tempDto)
    {
        if (tempDto is ButtonStateDto bDto)
        {
            SetLabel(bDto.Label);
            SetDisabled(bDto.IsDisabled);
            return true;
        }
        throw new ArgumentException("Invalid DTO type for AButton.");
    }

    public override CompStateDto GetState()
    {
        var dto = new ButtonStateDto
        {
            Label = GetLabel(),
            IsDisabled = GetDisabled()
        };
        return dto;
    }

    public override JsonTypeInfo GetJsonTypeInfo()
    {
        return ButtonSerializerContext.Default.ButtonStateDto;
    }
}