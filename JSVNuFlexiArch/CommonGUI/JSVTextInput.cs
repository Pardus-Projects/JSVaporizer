using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using NuFlexiArch;
using System.Runtime.Versioning;
using static JSVaporizer.JSVapor;

namespace JSVNuFlexiArch;

[SupportedOSPlatform("browser")]
public class JSVTextInput : ATextInput, IJSVComponent
{
    private IComponentRenderer _renderer;
    public string Id;
    public string LabelId;
    public JSVTextInput(string unqPrefix)
    {
        _renderer = new JSVTextInputRenderer();
        Metadata.Add("UnqPrefix", unqPrefix);
        Metadata.Add("CompTypeAQN", GetAssemblyQualifiedName());

        Id = JSVComponentHelpers.AppendElementSuffix(unqPrefix, "InputId");
        LabelId = JSVComponentHelpers.AppendElementSuffix(unqPrefix, "LabelId");
    }
    public IComponentRenderer GetRenderer() => _renderer;

    private string? _labelValue;
    private string? _textValue;

    public override void SetLabel(string? val = null)
    {
        _labelValue = val;
        Document.AssertGetElementById(LabelId).SetProperty("innerHTML", val??"");
    }

    public override string? GetLabel()
    {
        return _labelValue;
    }

    public override void SetInputValue(string? val)
    {
        _textValue = val;
        Document.AssertGetElementById(Id).SetFormElemValue(_textValue);
    }

    public override string? GetInputValue()
    {
        return _textValue;
    }
}

[SupportedOSPlatform("browser")]
public class JSVTextInputRenderer : JSVComponentRenderer
{
    protected override void RenderBody(AComponent tmpComp, HtmlContentBuilder htmlCB)
    {
        JSVTextInput comp = (JSVTextInput)tmpComp;

        string htmlStr = Environment.NewLine + $"<label id=\"{comp.LabelId}\" for=\"{comp.Id}\"></label>";
        htmlStr += Environment.NewLine + $"<input id=\"{comp.Id}\" type=\"text\"/>";

        htmlCB.AppendHtml(htmlStr);
    }
}


