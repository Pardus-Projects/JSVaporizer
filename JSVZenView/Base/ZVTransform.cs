using System.Runtime.Versioning;

namespace JSVZenView;

public abstract class TransformerDto;

public abstract class ZVTransform
{
    private string? _registryKey;

    public ZVTransform() { }

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
    private Dictionary<string, ZVTransform> _registry = new();

    public TransformerRegistry(Dictionary<string, ZVTransform> registry)
    {
        _registry = registry;
    }

    public ZVTransform Get(string xFormerRegistryKey)
    {
        if (_registry.ContainsKey(xFormerRegistryKey))
        {
            ZVTransform xFormer = _registry[xFormerRegistryKey];
            xFormer.SetRegistryKey(xFormerRegistryKey);
            return xFormer;
        }
        else
        {
            throw new Exception($"xFormerRegistryKey = \"{xFormerRegistryKey}\" does not exist in transformer registry.");
        }
    }

    [SupportedOSPlatform("browser")]
    public static string Invoke(TransformerRegistry transformerRegistry, string xFormerName, string dtoJson, string? userInfoJson = null)
    {
        ZVTransform xFormer = transformerRegistry.Get(xFormerName);
        string xFromRes = xFormer.DtoToView(dtoJson, userInfoJson);
        return xFromRes;
    }
}



