using System.Text.Json.Serialization.Metadata;
using System.Text.Json.Serialization;

namespace NuFlexiArch;

[JsonSerializable(typeof(SliderDataDto))]
public partial class SliderSerializerContext : JsonSerializerContext { }

public class SliderDataDto : CompDataDto
{
    public string? Label { get; set; }
    public double? Value { get; set; }
    public double MinValue { get; set; }
    public double MaxValue { get; set; }
    public double? Step { get; set; }
    public TextDisplayDataDto? ValueDisplayDto { get; set; } 
}

public abstract class ASlider : AComponent
{
    protected abstract void SetLabel(string? val);
    protected abstract string? GetLabel();

    protected abstract void SetValue(double? val);
    protected abstract double? GetValue();

    protected abstract void SetMinValue(double val);
    protected abstract double GetMinValue();

    protected abstract void SetMaxValue(double val);
    protected abstract double GetMaxValue();

    protected abstract void SetStep(double? val);
    protected abstract double? GetStep();

    public override bool UpdateState(CompDataDto tempDto)
    {
        if (tempDto is SliderDataDto sDto)
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

    public override CompDataDto GetState()
    {
        return new SliderDataDto
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
        return SliderSerializerContext.Default.SliderDataDto;
    }
}