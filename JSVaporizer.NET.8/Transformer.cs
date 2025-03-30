using System.Runtime.Versioning;

namespace JSVTransformer;

public class TransformerDto;

public abstract class Transformer
{
    private string? _registryKey;

    public Transformer() { }

    public abstract TransformerDto JsonToDto(string dtoJson, string? userInfoJson=null);

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
