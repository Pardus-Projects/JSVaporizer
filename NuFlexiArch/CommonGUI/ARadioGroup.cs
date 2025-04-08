using System.Text.Json.Serialization.Metadata;
using System.Text.Json.Serialization;

namespace NuFlexiArch;

[JsonSerializable(typeof(RadioGroupStateDto))]
public partial class RadioGroupSerializerContext : JsonSerializerContext { }

// Each option is a (value, label) pair; you might store them in the DTO.
public class RadioGroupOption
{
    public string? Value { get; set; }
    public string? Label { get; set; }
}

public class RadioGroupStateDto : CompStateDto
{
    public List<RadioGroupOption> Options { get; set; } = new();
    public string? SelectedValue { get; set; }
}

public abstract class ARadioGroup : AComponent
{
    public abstract void SetOptions(List<RadioGroupOption> options);
    public abstract List<RadioGroupOption> GetOptions();

    public abstract void SetSelectedValue(string? value);
    public abstract string? GetSelectedValue();

    public override bool SetState(CompStateDto tempDto)
    {
        if (tempDto is RadioGroupStateDto rgDto)
        {
            SetOptions(rgDto.Options);
            SetSelectedValue(rgDto.SelectedValue);
            return true;
        }
        throw new ArgumentException("Invalid DTO type for ARadioGroup.");
    }

    public override CompStateDto GetState()
    {
        var dto = new RadioGroupStateDto
        {
            Options = GetOptions(),
            SelectedValue = GetSelectedValue()
        };
        return dto;
    }

    public override JsonTypeInfo GetJsonTypeInfo()
    {
        return RadioGroupSerializerContext.Default.RadioGroupStateDto;
    }
}