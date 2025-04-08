using System.Text.Json.Serialization.Metadata;
using System.Text.Json.Serialization;

namespace NuFlexiArch;

[JsonSerializable(typeof(SliderStateDto))]
public partial class SliderSerializerContext : JsonSerializerContext { }

public class SliderStateDto : CompStateDto
{
    public string? Label { get; set; }
    public double? Value { get; set; }
    public double MinValue { get; set; }
    public double MaxValue { get; set; }
    public double? Step { get; set; }
    public TextDisplayDto? ValueDisplayDto { get; set; } 
}

public abstract class ASlider : AComponent
{
    public abstract void SetLabel(string? val);
    public abstract string? GetLabel();

    public abstract void SetValue(double? val);
    public abstract double? GetValue();

    public abstract void SetMinValue(double val);
    public abstract double GetMinValue();

    public abstract void SetMaxValue(double val);
    public abstract double GetMaxValue();

    public abstract void SetStep(double? val);
    public abstract double? GetStep();

    public override bool SetState(CompStateDto tempDto)
    {
        if (tempDto is SliderStateDto sDto)
        {
            SetLabel(sDto.Label);
            SetValue(sDto.Value);
            SetMinValue(sDto.MinValue);
            SetMaxValue(sDto.MaxValue);
            SetStep(sDto.Step);
            return true;
        }
        throw new ArgumentException("Invalid DTO type for ASlider.");
    }

    public override CompStateDto GetState()
    {
        return new SliderStateDto
        {
            Label = GetLabel(),
            Value = GetValue(),
            MinValue = GetMinValue(),
            MaxValue = GetMaxValue(),
            Step = GetStep()
        };
    }

    public override JsonTypeInfo GetJsonTypeInfo()
    {
        return SliderSerializerContext.Default.SliderStateDto;
    }
}