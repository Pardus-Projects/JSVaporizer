using JSVNuFlexiArch;
using System.Collections.Generic;
using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;

namespace MyViewLib;

[SupportedOSPlatform("browser")]
public static partial class MyTransformerRegistry
{
    private static TransformerRegistry _myTransformerReg = new(new Dictionary<string, JSVTransformer> {
        { "MyCoolTransformerV1", new BarberAppointmentTransformer() }
    });

    [JSExport]
    [SupportedOSPlatform("browser")]
    public static string Invoke(string xFormerName, string dtoJson, string? userInfoJson = null)
    {
        return TransformerRegistry.Invoke(_myTransformerReg, xFormerName, dtoJson, userInfoJson);
    }
}

