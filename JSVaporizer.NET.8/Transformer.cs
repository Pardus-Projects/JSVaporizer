using System;
using System.Collections.Generic;
using System.Runtime.Versioning;

namespace JSVaporizer;

public class TransformerDto;

public abstract class Transformer
{
    private string? _registryKey;

    public Transformer() { }

    public abstract TransformerDto JsonToDto(string dtoJson);

    public abstract string DtoToJson(TransformerDto dto);

    [SupportedOSPlatform("browser")]
    public abstract string DtoToView(string dtoJson, string? userInfoJson = null);

    [SupportedOSPlatform("browser")]
    public abstract TransformerDto ViewToDto();

    public void SetRegistryKey(string registryKey)
    {
        _registryKey = registryKey;
    }

    public string? GetRegistryKey()
    {
        return _registryKey;
    }
}

[SupportedOSPlatform("browser")]
public class TransformerRegistry
{
    private Dictionary<string, Transformer> _registry = new();

    public TransformerRegistry(Dictionary<string, Transformer> registry)
    {
        _registry = registry;
    }

    public Transformer Get(string xFormerRegistryKey)
    {
        if (_registry.ContainsKey(xFormerRegistryKey))
        {
            Transformer xFormer = _registry[xFormerRegistryKey];
            xFormer.SetRegistryKey(xFormerRegistryKey);
            return xFormer;
        }
        else
        {
            throw new Exception($"xFormerRegistryKey = \"{xFormerRegistryKey}\" does not exist in transformer registry.");
        }

    }
}

