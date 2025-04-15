using HandlebarsDotNet;
using Microsoft.AspNetCore.Html;
using System;

namespace JSVNuFlexiArch;

public abstract class JSVComponent
{
    public string UniqueName { get; }

    protected JSVComponent(string uniqueName)
    {
        UniqueName = uniqueName;
    }

    public string UniqueWithSuffix(string suffix)
    {
        return UniqueName + "_" + suffix;
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

    // For Handlebars templates.
    public override string ToString()
    {
        return Render();
    }

    public virtual string Render()
    {
        string htmlStr = RenderFromTemplate();
        return htmlStr;
    }

    private string RenderFromTemplate()
    {
        string hTemplate = GetTemplate();
        var template = Handlebars.Compile(hTemplate);
        return template(this);
    }

    public virtual HtmlContentBuilder GetHtmlContentBuilder()
    {
        HtmlContentBuilder htmlCB = new();
        htmlCB.AppendHtml(Render());
        return htmlCB;
    }

    protected virtual string GetTemplate()
    {
        return "";
    }
}

