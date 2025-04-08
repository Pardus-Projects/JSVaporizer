using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using NuFlexiArch;
using System.Runtime.Versioning;
using static JSVaporizer.JSVapor;

namespace JSVNuFlexiArch;

[SupportedOSPlatform("browser")]
public class JSVTextDisplay : ATextDisplay, IJSVComponent
{
    private IComponentRenderer _renderer;
    public string Id;

    public JSVTextDisplay(string unqPrefix)
    {
        _renderer = new JSVTextDisplayRenderer();
        Metadata.Add("UnqPrefix", unqPrefix);
        Metadata.Add("CompTypeAQN", GetAssemblyQualifiedName());

        Id = JSVComponentHelpers.AppendElementSuffix(unqPrefix, "InputId");
    }
    public IComponentRenderer GetRenderer() => _renderer;

    private string? _text;

    public override void SetText(string? text)
    {
        _text = text;
        Document.AssertGetElementById(Id).SetProperty("innerHTML", _text ?? "");
    }

    public override string? GetText()
    {
        throw new NotImplementedException();
    }
}

[SupportedOSPlatform("browser")]
public class JSVTextDisplayRenderer : JSVComponentRenderer
{
    protected override void RenderBody(AComponent tmpComp, HtmlContentBuilder htmlCB)
    {
        JSVTextDisplay comp = (JSVTextDisplay)tmpComp;

        string htmlStr = Environment.NewLine + $"<span id=\"{comp.Id}\"></span>";

        htmlCB.AppendHtml(htmlStr);
    }
}
