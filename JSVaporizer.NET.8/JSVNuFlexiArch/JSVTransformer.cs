using System;
using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;

namespace JSVNuFlexiArch;

public abstract class TransformerDto;

public abstract class JSVTransformer
{
    public string AssemblyQualifiedName { get; set; }

    public JSVTransformer()
    {
        AssemblyQualifiedName = GetAssemblyQualifiedName();
    }

    public abstract TransformerDto JsonToDto(string dtoJson);

    [SupportedOSPlatform("browser")]
    public abstract string DtoToView(string dtoJson, string? userInfoJson = null);

    [SupportedOSPlatform("browser")]
    public abstract TransformerDto ViewToDto();

    public abstract string DtoToJson(TransformerDto dto);

    public static JSVTransformer Instantiate(string assemblyQualifiedName)
    {
        Type? transformerType = Type.GetType(assemblyQualifiedName);
        if (transformerType == null)
        {
            throw new ArgumentException($"assemblyQualifiedName = \"{assemblyQualifiedName}\" was not found.");
        }

        JSVTransformer? xFormer = (JSVTransformer?)Activator.CreateInstance(transformerType);
        if (xFormer == null)
        {
            throw new ArgumentException($"CreateInstance() failed for transformerType = \"{transformerType}\"");
        }

        return xFormer;
    }

    [SupportedOSPlatform("browser")]
    public static string Invoke(string assemblyQualifiedName, string dtoJson, string? userInfoJson = null)
    {
        JSVTransformer xFormer = Instantiate(assemblyQualifiedName);
        string xFromRes = xFormer.DtoToView(dtoJson, userInfoJson);
        return xFromRes;
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

