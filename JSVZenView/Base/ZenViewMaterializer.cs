using Microsoft.AspNetCore.Html;
using System.Runtime.Versioning;
using System.Text.Encodings.Web;
using static JSVaporizer.JSVapor;

namespace JSVZenView;

[SupportedOSPlatform("browser")]
public static class ZenViewMaterializer
{
    public static bool Materialize(string metadataJson, string referenceElementId, bool append = false)
    {
        MetadataDto mdDto = MetadataDto.Deserialize(metadataJson);

        // Instantiate
        ZenView zView = ZenView.Instantiate(mdDto.UniqueName, mdDto.ViewTypeAQN);

        // Render, if not already rendered
        if (Document.GetElementById(zView.GetUniqueName()) == null)
        {
            Render(zView, referenceElementId, append);
        }

        return true;
    }

    public static bool Render(ZenView zView, string referenceElementId, bool append)
    {
        // Render HTML
        HtmlContentBuilder htmlCB = zView.RenderBuilder();

        // Put HTML in the DOM somewhere
        using (var sw = new StringWriter())
        {
            htmlCB.WriteTo(sw, HtmlEncoder.Default);
            string componentHtml = sw.ToString();

            Element referenceElem = Document.AssertGetElementById(referenceElementId);

            if (append)
            {
                referenceElem.AppendChild(Document.CreateElement(zView.GetUniqueName(), "div"));
                Document.AssertGetElementById(zView.GetUniqueName()).SetProperty("outerHTML", componentHtml);
            }
            else // replace
            {
                referenceElem.SetProperty("outerHTML", componentHtml);
            }
        }

        return true;
    }
}
