using JSVaporizer;
using JSVNuFlexiArch;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using NuFlexiArch;
using System;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using static JSVaporizer.JSVapor;

namespace MyViewLib;

[SupportedOSPlatform("browser")]
public class JSVTextInput : ATextInput, IJSVComponent
{
    private IComponentRenderer _renderer;
    public string InputId { get; set; }
    public string LabelId { get; set; }
    public JSVTextInput(string unqPrefix, IComponentRenderer renderer)
    {
        Metadata.Add("UnqPrefix", unqPrefix);
        Metadata.Add("CompTypeAQN", GetAssemblyQualifiedName());

        _renderer = renderer;
        InputId = JSVComponentHelpers.AppendElementSuffix(unqPrefix, "InputId");
        LabelId = JSVComponentHelpers.AppendElementSuffix(unqPrefix, "LabelId");
    }

    public override void SetLabelValue(string? val = null)
    {
        Document.AssertGetElementById(LabelId).SetProperty("innerHTML", val??"");
    }

    public override string? GetLabelValue()
    {
        ElementPropInfo propInfo =  Document.AssertGetElementById(LabelId).GetProperty("innerHTML");
        return propInfo.Value != null ? propInfo.Value.ToString() : "";
    }

    public override void SetInputValue(string? val)
    {
        Document.AssertGetElementById(InputId).SetFormElemValue(val);
    }

    public override string? GetInputValue()
    {
        return Document.AssertGetElementById(InputId).GetFormElemValue();
    }

    public IComponentRenderer GetRenderer()
    {
        return _renderer;
    }
}

[SupportedOSPlatform("browser")]
public class TextInputRenderer : JSVComponentRenderer
{
    protected override Task RenderBody(AComponent tmpComp, IHtmlHelper Html, HtmlContentBuilder htmlCB)
    {
        JSVTextInput comp = (JSVTextInput)tmpComp;

        string htmlStr = Environment.NewLine + $"<label id=\"{comp.LabelId}\" for=\"{comp.InputId}\">The input label</label>";
        htmlStr += Environment.NewLine + $"<input id=\"{comp.InputId}\" type=\"text\"/>";

        htmlCB.AppendHtml(htmlStr);

        return Task.CompletedTask;
    }
}


