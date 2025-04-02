using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;
using System.Collections.Generic;
using JSVaporizer;

namespace MyTransformerLib;

[SupportedOSPlatform("browser")]
public static partial class MyTransformerRegistry
{
    private static TransformerRegistry _jsvReg = new(new Dictionary<string, Transformer> { { "MyCoolTransformerV1", new MyCoolTransformer() } });

    public static Transformer Get(string xFormerRegistryKey)
    {
        return _jsvReg.Get(xFormerRegistryKey);
    }
}

public static partial class TransformerInvoker
{
    [SupportedOSPlatform("browser")]
    [JSExport]
    public static string Invoke(string xFormerName, string dtoJson)
    {
        Transformer xFormer = MyTransformerRegistry.Get(xFormerName);
        string xFromRes = xFormer.DtoToView(dtoJson);
        return xFromRes;
    }
}