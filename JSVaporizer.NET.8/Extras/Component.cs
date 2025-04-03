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
    public string UnqPrefix { get; }
    public string CompId { get; }
    public string DebugInfoId { get; }

    public List<SubCompDefinition> NamedSubComponents { get; set; }

    public Component(string unqPrefix)
    {
        UnqPrefix = unqPrefix;
        CompId = unqPrefix;
        DebugInfoId = AppendElementSuffix("DebugInfo");
        NamedSubComponents = new();
    }

    protected string MakeSubComponentUnqPrefix(string subPrefix)
    {
        return UnqPrefix + "_-_" + subPrefix;
    }

    protected string AppendElementSuffix(string suffix)
    {
        return UnqPrefix + "_" + suffix;
    }

    private void RenderOpen(HtmlContentBuilder htmlCB)
    {
        string compStart = Environment.NewLine + $"<div id=\"{CompId}\">";
        htmlCB.AppendHtml(compStart);

        htmlCB.AppendHtml(Environment.NewLine + $"<div id=\"{DebugInfoId}\">{DebugInfoId}</div>");
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
            string subCompPrefStr = MakeSubComponentUnqPrefix(subCompDef.SubPrefix);

            Type? compType = Type.GetType(subCompDef.CompType);
            if (compType == null)
            {
                throw new ArgumentException($"subCompDef.CompType = \"{subCompDef.CompType}\" was not found.");
            }

            Component? subComp = (Component?)Activator.CreateInstance(compType, new object[] { subCompPrefStr });
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
            compProps.List.Add(new(propInfo.Name, propInfo.GetValue(this)));
        }

        return JsonSerializer.Serialize(compProps, CompPropertiesContext.Default.CompProperties);
    }
}

[JsonSerializable(typeof(CompProperties))]
[JsonSerializable(typeof(SubCompDefinition))]
[JsonSerializable(typeof(List<SubCompDefinition>))]
[JsonSerializable(typeof(bool))]
[JsonSerializable(typeof(string))]
[JsonSerializable(typeof(int))]
[JsonSerializable(typeof(double))]
[JsonSerializable(typeof(decimal))]
public partial class CompPropertiesContext : JsonSerializerContext { }
public class CompProperties
{
    public List<CompProperty> List {get; set;} = new();
    public CompProperties() { }

    public Dictionary<string, object?> ToDictionary()
    {
        Dictionary<string, object?> dict = new();
        foreach (CompProperty prop in List)
        {
            dict[prop.Name] = prop.Value;
        }

        return dict;
    }
}

public class CompProperty
{
    public string Name { get; set; }
    public object? Value { get; set; }

    public CompProperty(string name, object? value)
    {
        Name = name;
        Value = value;
    }
}


