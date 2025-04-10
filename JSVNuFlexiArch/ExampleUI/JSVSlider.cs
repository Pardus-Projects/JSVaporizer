using JSVaporizer;
using Microsoft.AspNetCore.Html;
using JSVNuFlexiArch;
using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using static JSVaporizer.JSVapor;
using System.Diagnostics;
using System.Reflection.Emit;

namespace JSVNuFlexiArch;

[JsonSerializable(typeof(SliderDataDto))]
public partial class SliderSerializerContext : JsonSerializerContext { }

public class SliderDataDto : CompDataDto
{
    public string? Label { get; set; }
    public double? Value { get; set; }
    public double MinValue { get; set; }
    public double MaxValue { get; set; }
    public double? Step { get; set; }
}

[SupportedOSPlatform("browser")]
public class JSVSlider : JSVComponent
{
    private string? _labelValue;
    private double? _value;
    private double _minValue;
    private double _maxValue;
    private double? _step;

    private JSVTextDisplay _valueDisplay;
    private JSVButton _resetButton;

    public string Id;
    public string LabelId;

    public JSVSlider(string uniqueName) : base(uniqueName)
    {
        Id = uniqueName.AppendElementSuffix("InputId");
        LabelId = uniqueName.AppendElementSuffix("LabelId");

        _valueDisplay = new JSVTextDisplay(uniqueName.AppendSubComponentSuffix("ValueDisplay"));
        _resetButton = new JSVButton(uniqueName.AppendSubComponentSuffix("ResetButton"));
    }

    public override bool Initialize()
    {
        // Add change listener for slider
        Element thisSliderInput = Document.AssertGetElementById(Id);
        EventHandlerCalledFromJS changeHandler = (JSObject elem, string eventType, JSObject evnt) =>
        {
            string? valStr = thisSliderInput.GetFormElemValue();
            double val = 0;
            if (Double.TryParse(valStr, out val))
            {
                SetValue(val);
            }
            else
            {
                throw new JSVException($"Could not parse string \"{valStr}\"");
            }
            return (int)JSVEventHandlerBehavior.NoDefault_NoPropagate;
        };
        thisSliderInput.AddEventListener("change", JSVComponentHelpers.AppendElementSuffix(Id, "SliderChanged"), changeHandler);

        // Reset button
        _resetButton.UpdateState(new ButtonDataDto
        {
            Text = "Reset Me"
        });
        Element thisRestButton = Document.AssertGetElementById(_resetButton.Id);
        EventHandlerCalledFromJS resetClick = (JSObject elem, string eventType, JSObject evnt) =>
        {
            SetValue(GetMinValue());
            return (int)JSVEventHandlerBehavior.NoDefault_NoPropagate;
        };
        thisRestButton.AddEventListener("click", JSVComponentHelpers.AppendElementSuffix(Id, "ResetClicked"), resetClick);

        return true;
    }

    protected void SetLabel(string? val = null)
    {
        _labelValue = val;
        Document.AssertGetElementById(LabelId).SetProperty("innerHTML", val ?? "");
    }

    protected string? GetLabel()
    {
        return _labelValue;
    }

    protected void SetValue(double? val)
    {
        _value = val;
        Document.AssertGetElementById(Id).SetFormElemValue(_value);
        _valueDisplay.UpdateState(new TextDisplayDataDto()
        {
            Text = _value.ToString()
        });
    }

    protected double? GetValue()
    {
        return _value;
    }

    protected void SetMinValue(double val)
    {
        _minValue = val;
        Document.AssertGetElementById(Id).SetProperty("min", _minValue);
    }

    protected double GetMinValue()
    {
        return _minValue;
    }

    protected void SetMaxValue(double val)
    {
        _maxValue = val;
        Document.AssertGetElementById(Id).SetProperty("max", _maxValue);
    }

    protected double GetMaxValue()
    {
        return _maxValue;
    }

    protected void SetStep(double? val)
    {
        _step = val;
        if (_step != null)
        {
            Document.AssertGetElementById(Id).SetProperty("step", _step);
        }
    }

    protected double? GetStep()
    {
        return _step;
    }

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

    public override void RenderBody(HtmlContentBuilder htmlCB)
    {
        string htmlStr = Environment.NewLine + $"<label id=\"{LabelId}\" for=\"{Id}\"></label>";
        htmlStr += Environment.NewLine + $"<input id=\"{Id}\" type=\"range\"/>";
        htmlStr += Environment.NewLine + _valueDisplay.RenderBodyToHtml();
        htmlStr += Environment.NewLine + _resetButton.RenderBodyToHtml();
        htmlCB.AppendHtml(htmlStr);
    }
}

