using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;

namespace JSVaporizer;

public static partial class TransformerInvoker
{
    [SupportedOSPlatform("browser")]
    [JSExport]
    internal static string Invoke(string xFormerName, string dtoJson)
    {
        ITransformer xFormer = TransformerRegistry.Get(xFormerName);
        string xFromRes = xFormer.DtoToView(dtoJson);
        return xFromRes;
    }
}