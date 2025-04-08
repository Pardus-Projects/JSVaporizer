using JSVaporizer;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using NuFlexiArch;
using System.Runtime.Versioning;
using static JSVaporizer.JSVapor;

namespace JSVNuFlexiArch;

[SupportedOSPlatform("browser")]
public class JSVSlider : ASlider, IJSVComponent
{
    private IComponentRenderer _renderer;

    public string Id;
    public string LabelId;

    public JSVSlider(string unqPrefix)
    {
        _renderer = new JSVSliderRenderer();
        Metadata.Add("UnqPrefix", unqPrefix);
        Metadata.Add("CompTypeAQN", GetAssemblyQualifiedName());
        Id = JSVComponentHelpers.AppendElementSuffix(unqPrefix, "InputId");
        LabelId = JSVComponentHelpers.AppendElementSuffix(unqPrefix, "LabelId");
    }
    public IComponentRenderer GetRenderer() => _renderer;

    private string? _labelValue;
    private double? _value;
    private double _minValue;
    private double _maxValue;
    private double? _step;

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

        htmlCB.AppendHtml(htmlStr);
    }
}

