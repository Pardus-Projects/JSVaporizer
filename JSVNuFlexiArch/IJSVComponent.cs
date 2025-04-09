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
        string uniqueName = mdDict["UnqPrefix"];
        string compTypeAQN = mdDict["CompTypeAQN"];

        // Instantiate
        AComponent aComp = Instantiate(uniqueName, compTypeAQN);
        CompDataDto stateDto = aComp.DeserializeState(instanceDto.StateJson);

        // Render
        bool renderSuccess = Render(uniqueName, aComp, referenceElementId, append);

        // Set initialize
        bool initSuccess = aComp.Initialize();

        // Set state
        bool updateStateSuccess =  aComp.UpdateState(stateDto);

        return renderSuccess && initSuccess && updateStateSuccess;
    }

    public static AComponent Instantiate(string uniqueName, string compTypeAQN)
    {
        Type? compType = Type.GetType(compTypeAQN);
        if (compType == null)
        {
            throw new ArgumentException($"compTypeAQN = \"{compTypeAQN}\" was not found.");
        }

        AComponent? aComp = (AComponent?)Activator.CreateInstance(compType, new object[] { uniqueName });
        if (aComp == null)
        {
            throw new ArgumentException($"CreateInstance() failed for: uniqueName = \"{uniqueName}\", compType = \"{compType}\"");
        }

        return aComp;
    }

    public static bool Render(string uniqueName, AComponent aComp, string referenceElementId, bool append) {
        // Render HTML
        HtmlContentBuilder htmlCB = (HtmlContentBuilder) aComp.GetRenderer().Render(aComp);

        // Put HTML in the DOM somewhere
        using (var sw = new StringWriter())
        {
            htmlCB.WriteTo(sw, HtmlEncoder.Default);
            string componentHtml = sw.ToString();

            Element referenceElem = Document.AssertGetElementById(referenceElementId);

            if (append)
            {
                referenceElem.AppendChild(Document.CreateElement(uniqueName, "div"));
                Document.AssertGetElementById(uniqueName).SetProperty("outerHTML", componentHtml);
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
    public virtual string RenderBodyToHtml(AComponent comp)
    {
        HtmlContentBuilder htmlCB = new();
        RenderBody(comp, htmlCB);

        // Put HTML in the DOM somewhere
        using (var sw = new StringWriter())
        {
            htmlCB.WriteTo(sw, HtmlEncoder.Default);
            string componentHtml = sw.ToString();
            return componentHtml;
        }
    }

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
    public static string AppendElementSuffix(this string uniqueName, string suffix)
    {
        return uniqueName + "_" + suffix;
    }

    public static string AppendSubComponentuffix(this string uniqueName, string suffix)
    {
        return uniqueName + "_-_" + suffix;
    }
}





