using Microsoft.AspNetCore.Html;
using NuFlexiArch;
using System.Runtime.Versioning;
using System.Text.Encodings.Web;
using static JSVaporizer.JSVapor;

namespace JSVNuFlexiArch;

[SupportedOSPlatform("browser")]
public interface IJSVComponent : IComponent
{
    public static bool InstantiateAndRenderFromJson(string instanceDtoJson, string referenceElementId, bool append=false)
    {
        CompInstanceDto instanceDto = CompInstanceDto.Deserialize(instanceDtoJson);
        CompMetadata compMetadata = AComponent.DeserializeMetadata(instanceDto.MetadataJson);

        Dictionary<string, string> mdDict = AComponent.ToDictionary(compMetadata);
        string unqPrefix = mdDict["UnqPrefix"];
        string compTypeAQN = mdDict["CompTypeAQN"];

        // Instantiate
        AComponent aComp = Instantiate(unqPrefix, compTypeAQN);
        CompDataDto stateDto = aComp.DeserializeState(instanceDto.StateJson);

        // Render
        bool success = Render(unqPrefix, aComp, referenceElementId, append);

        // Set state
        return aComp.UpdateState(stateDto);
    }

    public static AComponent Instantiate(string unqPrefix, string compTypeAQN)
    {
        Type? compType = Type.GetType(compTypeAQN);
        if (compType == null)
        {
            throw new ArgumentException($"compTypeAQN = \"{compTypeAQN}\" was not found.");
        }

        AComponent? aComp = (AComponent?)Activator.CreateInstance(compType, new object[] { unqPrefix });
        if (aComp == null)
        {
            throw new ArgumentException($"CreateInstance() failed for: unqPrefix = \"{unqPrefix}\", compType = \"{compType}\"");
        }

        return aComp;
    }

    public static bool Render(string unqPrefix, AComponent aComp, string referenceElementId, bool append) {
        // Render HTML
        HtmlContentBuilder htmlCB = (HtmlContentBuilder)((IComponent)aComp).GetRenderer().Render(aComp);

        // Put HTML in the DOM somewhere
        using (var sw = new StringWriter())
        {
            htmlCB.WriteTo(sw, HtmlEncoder.Default);
            string componentHtml = sw.ToString();

            Element referenceElem = Document.AssertGetElementById(referenceElementId);

            if (append)
            {
                referenceElem.AppendChild(Document.CreateElement(unqPrefix, "div"));
                Document.AssertGetElementById(unqPrefix).SetProperty("outerHTML", componentHtml);
            }
            else // replace
            {
                referenceElem.SetProperty("outerHTML", componentHtml);
            }
        }

        return true;
    }
}

public class JSVComponentRenderer : IComponentRenderer
{
    private void RenderOpen(AComponent comp, HtmlContentBuilder htmlCB)
    {
        Dictionary<string, string> mdDict = AComponent.ToDictionary(comp.Metadata);

        string compStart = Environment.NewLine + $"<div id=\"{mdDict["UnqPrefix"]}\">";
        htmlCB.AppendHtml(compStart);
    }

    private void RenderClose(AComponent comp, HtmlContentBuilder htmlCB)
    {
        string compEnd = Environment.NewLine + $"</div>";
        htmlCB.AppendHtml(compEnd);
    }

    protected virtual void RenderBody(AComponent comp, HtmlContentBuilder htmlCB) { }

    public object Render(AComponent comp, params object[] args)
    {
        HtmlContentBuilder? htmlCB = new();
        if (args.Length > 0)
        {
            htmlCB = (HtmlContentBuilder)args[1];
            if (htmlCB == null)
            {
                htmlCB = new();
            }
        }

        RenderOpen(comp, htmlCB);
        RenderBody(comp, htmlCB);
        RenderClose(comp, htmlCB);

        return htmlCB;
    }
}

public static class JSVComponentHelpers
{
    public static string AppendElementSuffix(string unqPrefix, string suffix)
    {
        return unqPrefix + "_" + suffix;
    }
}





