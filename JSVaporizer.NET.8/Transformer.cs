using System.Runtime.Versioning;
using static JSVaporizer.JSVapor;

namespace JSVaporizer;

[SupportedOSPlatform("browser")]
public static partial class TransformerRegistry
{
    public static ITransformer Get(string xFormerRegistryKey)
    {
        var xFormerCreator = typeof(TransformerRegistry).GetMethod(xFormerRegistryKey);
        if (xFormerCreator != null)
        {
            ITransformer? xFormer = (ITransformer?)xFormerCreator.Invoke(null, null);
            if (xFormer == null)
            {
                throw new JSVException($"ITransformer for xFormerName = \"{xFormerRegistryKey}\" is null.");
            }
            xFormer.SetRegistryKey(xFormerRegistryKey);
            return xFormer;
        }
        else
        {
            throw new JSVException($"xFormerRegistryKey = \"{xFormerRegistryKey}\" does not exist in TransformerRegistry.");
        }
    }
}

public interface ITransformer
{
    public abstract TransformerDto? JsonToDto(string dtoJson);
    public abstract string? DtoToJson(TransformerDto dto);
    public abstract string DtoToView(string dtoJson);
    public abstract TransformerDto ViewToDto();

    public abstract void SetRegistryKey(string registryKey);
    public abstract string? GetRegistryKey();
}

public class TransformerDto;

public abstract class Transformer : ITransformer
{
    private string? _registryKey;

    public Transformer() { }

    public abstract TransformerDto JsonToDto(string dtoJson);

    public abstract string DtoToJson(TransformerDto dto);

    [SupportedOSPlatform("browser")]
    public abstract string DtoToView(string dtoJson);

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
