using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using NuFlexiArch;

namespace JSVNuFlexiArch;

public interface IJSVComponent : IComponent
{
    public static bool InitializeFromJson(string metadataJson, string stateDtoJson)
    {
        ComponentMetadata compMetadata = AComponent.DeserializeMetadata(metadataJson);
        Dictionary<string, string> mdDict = AComponent.ToDictionary(compMetadata);

        string unqPrefix = mdDict["UnqPrefix"];
        string compTypeAQN = mdDict["CompTypeAQN"];

        Type? compType = Type.GetType(compTypeAQN);
        if (compType == null)
        {
            throw new ArgumentException($"stateDto.CompType = \"{compTypeAQN}\" was not found.");
        }

        AComponent? aComp = (AComponent?)Activator.CreateInstance(compType, new object[] { unqPrefix, new BlackHole() });
        if (aComp == null)
        {
            throw new ArgumentException($"CreateInstance() failed for: compNameStr = \"{unqPrefix}\", compType = \"{compType}\"");
        }

        CompStateDto stateDto = aComp.DeserializeState(stateDtoJson);

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

    protected virtual Task RenderBody(AComponent comp, IHtmlHelper Html, HtmlContentBuilder htmlCB)
    {
        return Task.CompletedTask;
    }

    public async Task<object> RenderAsync(AComponent comp, params object[] args)
    {
        IHtmlHelper Html = (IHtmlHelper)args[0];
        HtmlContentBuilder? htmlCB = new();
        if (args.Length > 1)
        {
            htmlCB = (HtmlContentBuilder)args[1];
            if (htmlCB == null)
            {
                htmlCB = new();
            }
        }

        RenderOpen(comp, htmlCB);
        await RenderBody(comp, Html, htmlCB);
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





