using System.Text.Json.Serialization.Metadata;
using System.Text.Json.Serialization;

namespace NuFlexiArch;

[JsonSerializable(typeof(CheckboxStateDto))]
public partial class CheckboxSerializerContext : JsonSerializerContext { }

public class CheckboxStateDto : CompStateDto
{
    public string? Label { get; set; }
    public bool IsChecked { get; set; }
}

public abstract class ACheckbox : AComponent
{
    public abstract void SetLabel(string? label);
    public abstract string? GetLabel();

    public abstract void SetIsChecked(bool isChecked);
    public abstract bool GetIsChecked();

    public override bool SetState(CompStateDto tempDto)
    {
        if (tempDto is CheckboxStateDto dto)
        {
            SetLabel(dto.Label);
            SetIsChecked(dto.IsChecked);
            return true;
        }
        throw new ArgumentException("Invalid DTO type for ACheckbox.");
    }

    public override CompStateDto GetState()
    {
        return new CheckboxStateDto
        {
            Label = GetLabel(),
            IsChecked = GetIsChecked()
        };
    }

    public override JsonTypeInfo GetJsonTypeInfo()
    {
        return CheckboxSerializerContext.Default.CheckboxStateDto;
    }
}