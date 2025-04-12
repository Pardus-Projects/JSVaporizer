using Microsoft.AspNetCore.Html;
using System.Dynamic;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using HandlebarsDotNet;

namespace JSVZenView;

public abstract class ZenView
{
    private string _uniqueName;
    private string _viewTypeAQN;

    public string UniqueName {get { return GetUniqueName(); }}

    protected ZenView(string uniqueName)
    {
        _uniqueName = uniqueName;
        _viewTypeAQN = GetAssemblyQualifiedName();
    }

    public string GetMetadataJson()
    {
        MetadataDto mdDto = new MetadataDto
        {
            UniqueName = _uniqueName,
            ViewTypeAQN = _viewTypeAQN
        };

        return JsonSerializer.Serialize(mdDto, MetadataContext.Default.MetadataDto);
    }

    public string UniqueWithSuffix(string suffix)
    {
        return _uniqueName + "_" + suffix;
    }

    public string GetUniqueName()
    {
        return _uniqueName;
    }

    public string GetCompTypeAQN()
    {
        return _viewTypeAQN;
    }

    public static ZenView Instantiate(string uniqueName, string compTypeAQN)
    {
        Type? compType = Type.GetType(compTypeAQN);
        if (compType == null)
        {
            throw new ArgumentException($"compTypeAQN = \"{compTypeAQN}\" was not found.");
        }

        ZenView? aComp = (ZenView?)Activator.CreateInstance(compType, new object[] { uniqueName });
        if (aComp == null)
        {
            throw new ArgumentException($"CreateInstance() failed for: uniqueName = \"{uniqueName}\", compType = \"{compType}\"");
        }

        return aComp;
    }

    public override string ToString()
    {
        return Render();
    }

    public virtual string Render()
    {
        string hTemplate = GetTemplate();
        var template = Handlebars.Compile(hTemplate);
        string htmlStr = RenderFromTemplate(hTemplate);
        return htmlStr;
    }

    public virtual string RenderFromTemplate(string hTemplate)
    {
        Type compType = GetType();
        PropertyInfo[] props = GetType().GetProperties();
        var propsExpando = new ExpandoObject() as IDictionary<string, object>;
        foreach (PropertyInfo prop in props)
        {
            var propVal = prop.GetValue(this, null);
            propsExpando.Add(prop.Name, propVal??"".ToString());
        }
        var template = Handlebars.Compile(hTemplate);

        return template(propsExpando);
    }

    public virtual HtmlContentBuilder RenderBuilder()
    {
        HtmlContentBuilder htmlCB = new();
        htmlCB.AppendHtml(Render());
        return htmlCB;
    }

    protected virtual string GetTemplate()
    {
        return "";
    }

    private string GetAssemblyQualifiedName()
    {
        string? nFqn = GetType().AssemblyQualifiedName;
        if (nFqn == null)
        {
            throw new ArgumentNullException("nFqn is null");
        }
        return nFqn;
    }
}


[JsonSerializable(typeof(MetadataDto))]
public partial class MetadataContext : JsonSerializerContext { }
public class MetadataDto
{
    public required string UniqueName { get; set; }
    public required string ViewTypeAQN { get; set; }

    public static MetadataDto Deserialize(string compMetadataJson)
    {
        MetadataDto? nDto = JsonSerializer.Deserialize(compMetadataJson, MetadataContext.Default.MetadataDto);
        if (nDto == null)
        {
            throw new ArgumentException($"nDto = null for compMetadataJson = \"{compMetadataJson}\"");
        }
        return nDto;
    }
}
