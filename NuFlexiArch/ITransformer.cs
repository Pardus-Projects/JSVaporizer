namespace NuFlexiArch;

public abstract class TransformerDto;

public interface ITransformer
{
    public TransformerDto JsonToDto(string dtoJson);

    public string DtoToView(string dtoJson, string? userInfoJson = null);

    public TransformerDto ViewToDto();
    public string DtoToJson(TransformerDto dto);

    public void SetRegistryKey(string registryKey);

    public string? GetRegistryKey();
}

public interface ITransformerRegistry
{
    public ITransformer Get(string xFormerRegistryKey);

    public abstract static string Invoke(ITransformerRegistry transformerRegistry, string xFormerName, string dtoJson, string? userInfoJson = null);
}


