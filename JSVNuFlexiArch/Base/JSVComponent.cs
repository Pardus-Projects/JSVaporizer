using System.Text.Json.Serialization.Metadata;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Html;
using System.Reflection.Emit;

namespace JSVNuFlexiArch;

public delegate object? ComponentEventHandler(object[] args);

public abstract class JSVComponent
{
    protected JSVComponent(string uniqueName)
    {
        Renderer = new JSVComponentRenderer();
        Metadata.Add("UnqPrefix", uniqueName);
        Metadata.Add("CompTypeAQN", GetAssemblyQualifiedName());
    }

    public IJSVComponentRenderer Renderer { get; set; } = new BlackHole();
    public CompMetadata Metadata { get; set; } = new();

    public virtual bool Initialize() { return true; }

    public virtual bool UpdateState(CompDataDto stateDto) { return true; }

    public virtual CompDataDto GetState() { return new(); }

    public virtual JsonTypeInfo GetJsonTypeInfo()
    {
        return CompDataDtoContext.Default.CompDataDto;
    }

    public virtual string SerializeState(CompDataDto sDto)
    {
        return JsonSerializer.Serialize(sDto, GetJsonTypeInfo());
    }

    public virtual CompDataDto DeserializeState(string stateDtoJson)
    {
        CompDataDto? nDataDto = (CompDataDto?)JsonSerializer.Deserialize(stateDtoJson, GetJsonTypeInfo());
        if (nDataDto == null)
        {
            throw new ArgumentException($"nDataDto = null for stateDtoJson = \"{stateDtoJson}\"");
        }
        CompDataDto stateDto = nDataDto;
        return stateDto;
    }

    public string GetAssemblyQualifiedName()
    {
        string? nFqn = GetType().AssemblyQualifiedName;
        if (nFqn == null)
        {
            throw new ArgumentNullException("nFqn is null");
        }
        return nFqn;
    }

    public string SerializeMetadata()
    {
        return JsonSerializer.Serialize(Metadata, (JsonTypeInfo<CompMetadata>)MetadataContext.Default.CompMetadata);
    }

    public CompInstanceDto SerializeInstance(CompDataDto stateDto)
    {
        CompInstanceDto instanceDto = new()
        {
            MetadataJson = SerializeMetadata(),
            StateJson = SerializeState(stateDto)

        };
        return instanceDto;
    }

    public static CompMetadata DeserializeMetadata(string metadataJson)
    {
        CompMetadata? nCompMetadata = JsonSerializer.Deserialize(metadataJson, MetadataContext.Default.CompMetadata);
        if (nCompMetadata == null)
        {
            throw new ArgumentException($"nCompMetadata = null for stateDtoJson = \"{metadataJson}\"");
        }
        CompMetadata compMetadata = nCompMetadata;
        return compMetadata;
    }

    public static Dictionary<string, string> ToDictionary(CompMetadata md)
    {
        Dictionary<string, string> dict = new();
        foreach (CompMetadataItem mdItem in md.List)
        {
            dict[mdItem.Name] = mdItem.Value;
        }

        return dict;
    }

    public virtual void RenderBody(HtmlContentBuilder htmlCB) { }
}

[JsonSerializable(typeof(CompInstanceDto))]
public partial class ComponentInstanceContext : JsonSerializerContext { }
public class CompInstanceDto
{
    public required string MetadataJson { get; set; }
    public required string StateJson { get; set; }

    public string Serialize()
    {
        return JsonSerializer.Serialize(this, (JsonTypeInfo<CompInstanceDto>)ComponentInstanceContext.Default.CompInstanceDto);
    }

    public static CompInstanceDto Deserialize(string instanceDtoJson)
    {
        CompInstanceDto? nCompInstanceDto = JsonSerializer.Deserialize(instanceDtoJson, ComponentInstanceContext.Default.CompInstanceDto);
        if (nCompInstanceDto == null)
        {
            throw new ArgumentException($"nCompInstanceDto = null for instanceDtoJson = \"{instanceDtoJson}\"");
        }
        CompInstanceDto instanceDto = nCompInstanceDto;
        return instanceDto;
    }
}

[JsonSerializable(typeof(CompMetadata))]
[JsonSerializable(typeof(List<CompMetadataItem>))]
[JsonSerializable(typeof(CompMetadataItem))]
public partial class MetadataContext : JsonSerializerContext { }
public class CompMetadata
{
    public List<CompMetadataItem> List { get; set; } = new();
    public void Add(string name, string value)
    {
        List.Add(new CompMetadataItem(name, value));
    }
}

public class CompMetadataItem
{
    public string Name { get; set; }
    public string Value { get; set; }
    public CompMetadataItem(string name, string value)
    {
        Name = name;
        Value = value;
    }
}

[JsonSerializable(typeof(CompDataDto))]
public partial class CompDataDtoContext : JsonSerializerContext { }
public class CompDataDto { }




