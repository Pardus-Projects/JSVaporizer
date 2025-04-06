using System.Text.Json.Serialization.Metadata;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NuFlexiArch;


[JsonSerializable(typeof(ComponentMetadata))]
[JsonSerializable(typeof(List<CompMetadataItem>))]
[JsonSerializable(typeof(CompMetadataItem))]
public partial class MetadataContext : JsonSerializerContext { }
public class ComponentMetadata
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

[JsonSerializable(typeof(CompStateDto))]
public partial class CompStateDtoContext : JsonSerializerContext { }
public class CompStateDto { }

public abstract class AComponent
{
    public ComponentMetadata Metadata { get; set; } = new();

    public virtual bool SetState(CompStateDto stateDto) { return true; }

    public virtual CompStateDto GetState() { return new(); }

    public virtual JsonTypeInfo GetJsonTypeInfo()
    {
        return CompStateDtoContext.Default.CompStateDto;
    }

    public virtual string SerializeState(CompStateDto sDto)
    {
        return JsonSerializer.Serialize(sDto, GetJsonTypeInfo());
    }

    public virtual CompStateDto DeserializeState(string stateDtoJson)
    {
        CompStateDto? nStateDto = (CompStateDto?)JsonSerializer.Deserialize(stateDtoJson, GetJsonTypeInfo());
        if (nStateDto == null)
        {
            throw new ArgumentException($"nStateDto = null for stateDtoJson = \"{stateDtoJson}\"");
        }
        CompStateDto stateDto = nStateDto;
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
        return JsonSerializer.Serialize(Metadata, MetadataContext.Default.ComponentMetadata);
    }

    public static ComponentMetadata DeserializeMetadata(string metadataJson)
    {
        ComponentMetadata? nCompMetadata = JsonSerializer.Deserialize(metadataJson, MetadataContext.Default.ComponentMetadata);
        if (nCompMetadata == null)
        {
            throw new ArgumentException($"nCompMetadata = null for stateDtoJson = \"{metadataJson}\"");
        }
        ComponentMetadata compMetadata = nCompMetadata;
        return compMetadata;
    }

    public static Dictionary<string, string> ToDictionary(ComponentMetadata md)
    {
        Dictionary<string, string> dict = new();
        foreach (CompMetadataItem mdItem in md.List)
        {
            dict[mdItem.Name] = mdItem.Value;
        }

        return dict;
    }
}