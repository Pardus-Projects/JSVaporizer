using Microsoft.AspNetCore.Html;
using System.Runtime.Versioning;
using System.Text.Encodings.Web;
using static JSVaporizer.JSVapor;

namespace JSVNuFlexiArch;

[SupportedOSPlatform("browser")]
public static class JSVComponentMaterializer
{
    public static bool InstantiateAndRenderFromJson(string instanceDtoJson, string referenceElementId, bool append = false)
    {
        CompInstanceDto instanceDto = CompInstanceDto.Deserialize(instanceDtoJson);
        CompMetadata compMetadata = JSVComponent.DeserializeMetadata(instanceDto.MetadataJson);

        Dictionary<string, string> mdDict = JSVComponent.ToDictionary(compMetadata);
        string uniqueName = mdDict["UnqPrefix"];
        string compTypeAQN = mdDict["CompTypeAQN"];

        // Instantiate
        JSVComponent aComp = Instantiate(uniqueName, compTypeAQN);
        CompDataDto stateDto = aComp.DeserializeState(instanceDto.StateJson);

        // Render
        bool renderSuccess = Render(uniqueName, aComp, referenceElementId, append);

        // Set initialize
        bool initSuccess = aComp.Initialize();

        // Set state
        bool updateStateSuccess = aComp.UpdateState(stateDto);

        return renderSuccess && initSuccess && updateStateSuccess;
    }

    public static JSVComponent Instantiate(string uniqueName, string compTypeAQN)
    {
        Type? compType = Type.GetType(compTypeAQN);
        if (compType == null)
        {
            throw new ArgumentException($"compTypeAQN = \"{compTypeAQN}\" was not found.");
        }

        JSVComponent? aComp = (JSVComponent?)Activator.CreateInstance(compType, new object[] { uniqueName });
        if (aComp == null)
        {
            throw new ArgumentException($"CreateInstance() failed for: uniqueName = \"{uniqueName}\", compType = \"{compType}\"");
        }

        return aComp;
    }

    public static bool Render(string uniqueName, JSVComponent aComp, string referenceElementId, bool append)
    {
        // Render HTML
        HtmlContentBuilder htmlCB = (HtmlContentBuilder)aComp.Renderer.Render(aComp);

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
