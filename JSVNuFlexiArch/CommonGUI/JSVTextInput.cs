using Microsoft.AspNetCore.Html;
using NuFlexiArch;
using System.Runtime.Versioning;
using static JSVaporizer.JSVapor;

namespace JSVNuFlexiArch;

[SupportedOSPlatform("browser")]
public class JSVTextInput : ATextInput, IJSVComponent
{
    public string Id;
    public string LabelId;
    public JSVTextInput(string uniqueName)
    {
        Renderer = new JSVTextInputRenderer();
        Metadata.Add("UnqPrefix", uniqueName);
        Metadata.Add("CompTypeAQN", GetAssemblyQualifiedName());

        Id = uniqueName.AppendElementSuffix("InputId");
        LabelId = uniqueName.AppendElementSuffix("LabelId");
    }

    private string? _labelValue;
    private string? _textValue;

    protected override void SetLabel(string? val = null)
    {
        _labelValue = val;
        Document.AssertGetElementById(LabelId).SetProperty("innerHTML", val??"");
    }

    protected override string? GetLabel()
    {
        return _labelValue;
    }

    protected override void SetInputValue(string? val)
    {
        _textValue = val;
        Document.AssertGetElementById(Id).SetFormElemValue(_textValue);
    }

    protected override string? GetInputValue()
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


