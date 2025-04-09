using System.Text.Json.Serialization.Metadata;
using System.Text.Json.Serialization;

namespace NuFlexiArch;

[JsonSerializable(typeof(CheckboxDataDto))]
public partial class CheckboxSerializerContext : JsonSerializerContext { }

public class CheckboxDataDto : CompDataDto
{
    public string? Label { get; set; }
    public bool IsChecked { get; set; }
}

public abstract class ACheckbox : AComponent
{
    protected abstract void SetLabel(string? label);
    protected abstract string? GetLabel();

    protected abstract void SetIsChecked(bool isChecked);
    protected abstract bool GetIsChecked();

    public override bool UpdateState(CompDataDto tempDto)
    {
        if (tempDto is CheckboxDataDto dto)
        {
            SetLabel(dto.Label);
            SetIsChecked(dto.IsChecked);
            return true;
        }
        throw new ArgumentException("Invalid DTO type for ACheckbox.");
    }

    public override CompDataDto GetState()
    {
        return new CheckboxDataDto
        {
            Label = GetLabel(),
            IsChecked = GetIsChecked()
        };
    }

    public override JsonTypeInfo GetJsonTypeInfo()
    {
        return CheckboxSerializerContext.Default.CheckboxDataDto;
    }
}