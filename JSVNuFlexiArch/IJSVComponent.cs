using Microsoft.AspNetCore.Html;
using NuFlexiArch;
using System.Runtime.Versioning;
using System.Text.Encodings.Web;
using static JSVaporizer.JSVapor;

namespace JSVNuFlexiArch;

[SupportedOSPlatform("browser")]
public interface IJSVComponent : IComponent
{
    public static bool InstantiateFromJson(string instanceDtoJson, string referenceElementId, bool append=false)
    {
        ComponentInstanceDto instanceDto = ComponentInstanceDto.Deserialize(instanceDtoJson);
        string metadataJson = instanceDto.MetadataJson;
        string stateDtoJson = instanceDto.StateJson;

        ComponentMetadata compMetadata = AComponent.DeserializeMetadata(metadataJson);
        Dictionary<string, string> mdDict = AComponent.ToDictionary(compMetadata);

        string unqPrefix = mdDict["UnqPrefix"];
        string compTypeAQN = mdDict["CompTypeAQN"];

        Type? compType = Type.GetType(compTypeAQN);
        if (compType == null)
        {
            throw new ArgumentException($"stateDto.CompType = \"{compTypeAQN}\" was not found.");
        }

        AComponent? aComp = (AComponent?)Activator.CreateInstance(compType, new object[] { unqPrefix });
        if (aComp == null)
        {
            throw new ArgumentException($"CreateInstance() failed for: unqPrefix = \"{unqPrefix}\", compType = \"{compType}\"");
        }

        CompStateDto stateDto = aComp.DeserializeState(stateDtoJson);

        // Render
        HtmlContentBuilder htmlCB = (HtmlContentBuilder)((IComponent)aComp).GetRenderer().Render(aComp);
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

        return aComp.SetState(stateDto);
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





