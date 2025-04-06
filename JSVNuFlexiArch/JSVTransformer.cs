using NuFlexiArch;
using System.Runtime.Versioning;

namespace JSVNuFlexiArch;

public abstract class JSVTransformer : ITransformer
{
    private string? _registryKey;

    public JSVTransformer() { }

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

public class TransformerRegistry : ITransformerRegistry
{
    private Dictionary<string, JSVTransformer> _registry = new();

    public TransformerRegistry(Dictionary<string, JSVTransformer> registry)
    {
        _registry = registry;
    }

    public ITransformer Get(string xFormerRegistryKey)
    {
        if (_registry.ContainsKey(xFormerRegistryKey))
        {
            JSVTransformer xFormer = _registry[xFormerRegistryKey];
            xFormer.SetRegistryKey(xFormerRegistryKey);
            return xFormer;
        }
        else
        {
            throw new Exception($"xFormerRegistryKey = \"{xFormerRegistryKey}\" does not exist in transformer registry.");
        }
    }

    [SupportedOSPlatform("browser")]
    public static string Invoke(ITransformerRegistry transformerRegistry, string xFormerName, string dtoJson, string? userInfoJson = null)
    {
        ITransformer xFormer = transformerRegistry.Get(xFormerName);
        string xFromRes = xFormer.DtoToView(dtoJson, userInfoJson);
        return xFromRes;
    }
}



