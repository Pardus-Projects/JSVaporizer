using Microsoft.AspNetCore.Html;
using System.Text.Encodings.Web;

namespace JSVNuFlexiArch;

public interface IJSVComponentRenderer
{
    public HtmlContentBuilder Render(JSVComponent comp);
}

public class BlackHole : IJSVComponentRenderer
{
    public HtmlContentBuilder Render(JSVComponent comp)
    {
        return new();
    }
}

public class JSVComponentRenderer : IJSVComponentRenderer
{
    private static void RenderOpen(JSVComponent comp, HtmlContentBuilder htmlCB)
    {
        Dictionary<string, string> mdDict = JSVComponent.ToDictionary(comp.Metadata);

        string compStart = Environment.NewLine + $"<div id=\"{mdDict["UnqPrefix"]}\">";
        htmlCB.AppendHtml(compStart);
    }

    private static void RenderClose(HtmlContentBuilder htmlCB)
    {
        string compEnd = Environment.NewLine + $"</div>";
        htmlCB.AppendHtml(compEnd);
    }

    public virtual HtmlContentBuilder Render(JSVComponent comp)
    {
        HtmlContentBuilder? htmlCB = new();
        RenderOpen(comp, htmlCB);
        comp.RenderBody(htmlCB);
        RenderClose(htmlCB);

        return htmlCB;
    }
}







