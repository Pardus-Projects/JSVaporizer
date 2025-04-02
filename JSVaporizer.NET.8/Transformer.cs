using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;

namespace JSVaporizer;

public class TransformerDto;

public abstract class Transformer
{
    private string? _registryKey;

    public Transformer() { }

    public abstract TransformerDto JsonToDto(string dtoJson);

    [SupportedOSPlatform("browser")]
    public abstract string DtoToView(string dtoJson, string? userInfoJson = null);

    [SupportedOSPlatform("browser")]
    public abstract TransformerDto ViewToDto();
    public abstract string DtoToJson(TransformerDto dto);

    public void SetRegistryKey(string registryKey)
    {
        _registryKey = registryKey;
    }

    public string? GetRegistryKey()
    {
        return _registryKey;
    }
}

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

public static partial class TransformerInvoker
{
    [SupportedOSPlatform("browser")]
    public static string Invoke(TransformerRegistry transformerRegistry, string xFormerName, string dtoJson, string? userInfoJson = null)
    {
        Transformer xFormer = transformerRegistry.Get(xFormerName);
        string xFromRes = xFormer.DtoToView(dtoJson, userInfoJson);
        return xFromRes;
    }
}


