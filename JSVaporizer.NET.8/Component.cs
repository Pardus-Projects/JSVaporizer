using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace JSVComponent;

public class SubCompDefinition
{
    public string SubPrefix { get; set; }

    public string CompType { get; set; }

    public SubCompDefinition(string subPrefix, Type type)
    {
        if (type.AssemblyQualifiedName == null)
        {
            throw new ArgumentException("Cannot get full name for " + type.ToString());
        }

        SubPrefix = subPrefix;
        CompType = type.AssemblyQualifiedName;
    }
}

public class Component
{
    public static bool DebugMode { get; set; } = false;

    public string UnqPrefix { get; }
    public string CompId { get; }

    public List<SubCompDefinition> NamedSubComponents { get; set; }

    public Component(string unqPrefix)
    {
        UnqPrefix = unqPrefix;
        CompId = unqPrefix;
        NamedSubComponents = new();
    }

    protected string PrependUnqPrefix(string subPrefix)
    {
        return UnqPrefix + "_-_" + subPrefix;
    }

    private void RenderOpen(HtmlContentBuilder htmlCB)
    {
        string compStart = Environment.NewLine + $"<div id=\"{CompId}\">";
        htmlCB.AppendHtml(compStart);

        if (DebugMode)
        {
            htmlCB.AppendHtml(Environment.NewLine + $"CompId=\"{CompId}\"");
        }
    }

    private void RenderClose(HtmlContentBuilder htmlCB)
    {
        string compEnd = Environment.NewLine + $"</div>";
        htmlCB.AppendHtml(compEnd);
    }

    protected virtual Task RenderBody(IHtmlHelper Html, HtmlContentBuilder htmlCB)
    {
        return Task.CompletedTask;
    }

    private async Task RenderNamedSubComponents(IHtmlHelper Html, HtmlContentBuilder htmlCB)
    {
        foreach (SubCompDefinition subCompDef in NamedSubComponents)
        {
            string subCompPrefStr = PrependUnqPrefix(subCompDef.SubPrefix);

            Type? compType = Type.GetType(subCompDef.CompType);
            if (compType == null)
            {
                throw new ArgumentException($"subCompDef.CompType = \"{subCompDef.CompType}\" was not found.");
            }

            Component? subComp = (Component?)Activator.CreateInstance(compType, new object[] { PrependUnqPrefix(subCompPrefStr) });
            if (subComp == null)
            {
                throw new ArgumentException($"CreateInstance() failed for: compNameStr = \"{subCompPrefStr}\", compType = \"{compType}\"");
            }
            await subComp.RenderAsync(Html, htmlCB);
        }
    }

    public async Task<IHtmlContent> RenderAsync(IHtmlHelper Html, HtmlContentBuilder? htmlCB = null)
    {
        if (htmlCB == null)
        {
            htmlCB = new();
        }
        RenderOpen(htmlCB);
        await RenderBody(Html, htmlCB);
        await RenderNamedSubComponents(Html, htmlCB);
        RenderClose(htmlCB);

        return htmlCB;
    }

    public string SerializeComponentProperties()
    {
        CompProperties compProps = new();
        PropertyInfo[] properties = GetType().GetProperties();
        foreach (PropertyInfo propInfo in properties)
        {
            compProps.List.Add(new() { Name = propInfo.Name, Value = propInfo.GetValue(this) });
        }

        return JsonSerializer.Serialize(compProps, CompPropertiesContext.Default.CompProperties);
    }
}

[JsonSerializable(typeof(CompProperties))]
[JsonSerializable(typeof(SubCompDefinition))]
[JsonSerializable(typeof(List<SubCompDefinition>))]
public partial class CompPropertiesContext : JsonSerializerContext { }
public class CompProperties
{
    public List<CompProperty> List {get; set;} = new();
    public CompProperties() { }
}

public class CompProperty
{
    public string? Name { get; set; }
    public object? Value { get; set; }
}


