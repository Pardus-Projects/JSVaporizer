using JSVaporizer;
using Microsoft.AspNetCore.Html;
using NuFlexiArch;
using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;
using static JSVaporizer.JSVapor;

namespace JSVNuFlexiArch;

[SupportedOSPlatform("browser")]
public class JSVSlider : ASlider, IJSVComponent
{
    private string? _labelValue;
    private double? _value;
    private double _minValue;
    private double _maxValue;
    private double? _step;

    public string Id;
    public string LabelId;
    public JSVTextDisplay ValueDisplay;

    public JSVSlider(string uniqueName)
    {
        Renderer = new JSVSliderRenderer();
        Metadata.Add("UnqPrefix", uniqueName);
        Metadata.Add("CompTypeAQN", GetAssemblyQualifiedName());

        Id = uniqueName.AppendElementSuffix("InputId");
        LabelId = uniqueName.AppendElementSuffix("LabelId");

        ValueDisplay = new JSVTextDisplay(uniqueName.AppendSubComponentuffix("ValueDisplay"));
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
        thisSliderInput.AddEventListener("change", $"{Id}_OnChange", changeHandler);
        return true;
    }

    protected override void SetLabel(string? val = null)
    {
        _labelValue = val;
        Document.AssertGetElementById(LabelId).SetProperty("innerHTML", val ?? "");
    }

    protected override string? GetLabel()
    {
        return _labelValue;
    }

    protected override void SetValue(double? val)
    {
        _value = val;
        Document.AssertGetElementById(Id).SetFormElemValue(_value);
        ValueDisplay.UpdateState(new TextDisplayDataDto()
        {
            Text = _value.ToString()
        });
    }

    protected override double? GetValue()
    {
        return _value;
    }

    protected override void SetMinValue(double val)
    {
        _minValue = val;
        Document.AssertGetElementById(Id).SetProperty("min", _minValue);
    }

    protected override double GetMinValue()
    {
        return _minValue;
    }

    protected override void SetMaxValue(double val)
    {
        _maxValue = val;
        Document.AssertGetElementById(Id).SetProperty("max", _maxValue);
    }

    protected override double GetMaxValue()
    {
        return _maxValue;
    }

    protected override void SetStep(double? val)
    {
        _step = val;
        if (_step != null)
        {
            Document.AssertGetElementById(Id).SetProperty("step", _step);
        }
    }

    protected override double? GetStep()
    {
        return _step;
    }
}

[SupportedOSPlatform("browser")]
public class JSVSliderRenderer : JSVComponentRenderer
{
    protected override void RenderBody(AComponent tmpComp, HtmlContentBuilder htmlCB)
    {
        JSVSlider comp = (JSVSlider)tmpComp;

        string htmlStr = Environment.NewLine + $"<label id=\"{comp.LabelId}\" for=\"{comp.Id}\"></label>";
        htmlStr += Environment.NewLine + $"<input id=\"{comp.Id}\" type=\"range\"/>";
        htmlStr += Environment.NewLine + ((JSVComponentRenderer) comp.ValueDisplay.Renderer).RenderBodyToHtml(comp.ValueDisplay);
        htmlCB.AppendHtml(htmlStr);
    }
}

