using Microsoft.AspNetCore.Html;
using NuFlexiArch;
using System.Runtime.Versioning;
using static JSVaporizer.JSVapor;

namespace JSVNuFlexiArch;

[SupportedOSPlatform("browser")]
public class JSVTextDisplay : ATextDisplay, IJSVComponent
{
    public string Id;

    public JSVTextDisplay(string uniqueName)
    {
        Renderer = new JSVTextDisplayRenderer();
        Metadata.Add("UnqPrefix", uniqueName);
        Metadata.Add("CompTypeAQN", GetAssemblyQualifiedName());

        Id = uniqueName.AppendElementSuffix("InputId");
    }

    private string? _text;

    protected override void SetText(string? text)
    {
        _text = text;
        Document.AssertGetElementById(Id).SetProperty("innerHTML", _text ?? "");
    }

    protected override string? GetText()
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
