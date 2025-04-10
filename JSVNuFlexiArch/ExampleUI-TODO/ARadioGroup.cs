using System.Text.Json.Serialization.Metadata;
using System.Text.Json.Serialization;

namespace JSVNuFlexiArch;

[JsonSerializable(typeof(RadioGroupDataDto))]
public partial class RadioGroupSerializerContext : JsonSerializerContext { }

// Each option is a (value, label) pair; you might store them in the DTO.
public class RadioGroupOption
{
    public string? Value { get; set; }
    public string? Label { get; set; }
}

public class RadioGroupDataDto : CompDataDto
{
    public List<RadioGroupOption> Options { get; set; } = new();
    public string? SelectedValue { get; set; }
}

public abstract class ARadioGroup : JSVComponent
{
    protected ARadioGroup(string uniqueName) : base(uniqueName) { }

    protected abstract void SetOptions(List<RadioGroupOption> options);
    protected abstract List<RadioGroupOption> GetOptions();

    protected abstract void SetSelectedValue(string? value);
    protected abstract string? GetSelectedValue();

    public override bool UpdateState(CompDataDto tempDto)
    {
        if (tempDto is RadioGroupDataDto rgDto)
        {
            SetOptions(rgDto.Options);
            SetSelectedValue(rgDto.SelectedValue);
            return true;
        }
        throw new ArgumentException("Invalid DTO type for ARadioGroup.");
    }

    public override CompDataDto GetState()
    {
        var dto = new RadioGroupDataDto
        {
            Options = GetOptions(),
            SelectedValue = GetSelectedValue()
        };
        return dto;
    }

    public override JsonTypeInfo GetJsonTypeInfo()
    {
        return RadioGroupSerializerContext.Default.RadioGroupDataDto;
    }
}