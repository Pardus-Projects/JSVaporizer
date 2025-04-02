using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;
using System.Collections.Generic;
using JSVaporizer;

namespace MyTransformerLib;

[SupportedOSPlatform("browser")]
public static partial class MyTransformerRegistry
{
    private static TransformerRegistry _myTransformerReg = new(new Dictionary<string, Transformer> {
        { "MyExampleCompTransformer", new MyExampleCompTransformer() },
        { "MyCoolTransformerV1", new MyCoolTransformer() }
    });

    [JSExport]
    [SupportedOSPlatform("browser")]
    public static string Invoke(string xFormerName, string dtoJson, string? userInfoJson = null)
    {
        return TransformerInvoker.Invoke(_myTransformerReg, xFormerName, dtoJson, userInfoJson);
    }
}

