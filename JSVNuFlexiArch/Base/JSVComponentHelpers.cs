using Microsoft.AspNetCore.Html;
using System.Text.Encodings.Web;

namespace JSVNuFlexiArch;

public static class JSVComponentHelpers
{
    public static string AppendElementSuffix(this string uniqueName, string suffix)
    {
        return uniqueName + "_" + suffix;
    }

    public static string AppendSubComponentSuffix(this string uniqueName, string suffix)
    {
        return uniqueName + "_-_" + suffix;
    }

    public static string RenderBodyToHtml(this JSVComponent comp)
    {
        HtmlContentBuilder htmlCB = new();
        comp.RenderBody(htmlCB);
        using (var sw = new StringWriter())
        {
            htmlCB.WriteTo(sw, HtmlEncoder.Default);
            string componentHtml = sw.ToString();
            return componentHtml;
        }
    }
}
