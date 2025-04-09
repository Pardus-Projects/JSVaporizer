using System.Text.Json.Serialization.Metadata;
using System.Text.Json.Serialization;

namespace NuFlexiArch;

[JsonSerializable(typeof(ButtonDataDto))]
public partial class ButtonSerializerContext : JsonSerializerContext { }

public class ButtonDataDto : CompDataDto
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

    public override bool UpdateState(CompDataDto tempDto)
    {
        if (tempDto is ButtonDataDto bDto)
        {
            SetLabel(bDto.Label);
            SetDisabled(bDto.IsDisabled);
            return true;
        }
        throw new ArgumentException("Invalid DTO type for AButton.");
    }

    public override CompDataDto GetState()
    {
        var dto = new ButtonDataDto
        {
            Label = GetLabel(),
            IsDisabled = GetDisabled()
        };
        return dto;
    }

    public override JsonTypeInfo GetJsonTypeInfo()
    {
        return ButtonSerializerContext.Default.ButtonDataDto;
    }
}