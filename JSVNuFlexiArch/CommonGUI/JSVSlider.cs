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

    public override void SetLabel(string? val = null)
    {
        _labelValue = val;
        Document.AssertGetElementById(LabelId).SetProperty("innerHTML", val ?? "");
    }

    public override string? GetLabel()
    {
        return _labelValue;
    }

    public override void SetValue(double? val)
    {
        _value = val;
        Document.AssertGetElementById(Id).SetFormElemValue(_value);

        ValueDisplay.SetText(_value.ToString());
    }

    public override double? GetValue()
    {
        return _value;
    }

    public override void SetMinValue(double val)
    {
        _minValue = val;
        Document.AssertGetElementById(Id).SetProperty("min", _minValue);
    }

    public override double GetMinValue()
    {
        return _minValue;
    }

    public override void SetMaxValue(double val)
    {
        _maxValue = val;
        Document.AssertGetElementById(Id).SetProperty("max", _maxValue);
    }

    public override double GetMaxValue()
    {
        return _maxValue;
    }

    public override void SetStep(double? val)
    {
        _step = val;
        if (_step != null)
        {
            Document.AssertGetElementById(Id).SetProperty("step", _step);
        }
    }

    public override double? GetStep()
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

