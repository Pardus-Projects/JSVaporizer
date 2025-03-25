using System;
using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;

namespace JSVTransformer;

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
                throw new System.Exception($"ITransformer for xFormerName = \"{xFormerRegistryKey}\" is null.");
            }
            xFormer.SetRegistryKey(xFormerRegistryKey);
            return xFormer;
        }
        else
        {
            throw new System.Exception($"xFormerRegistryKey = \"{xFormerRegistryKey}\" does not exist in TransformerRegistry.");
        }
    }
}
public static partial class TransformerInvoker
{
    [SupportedOSPlatform("browser")]
    [JSExport]
    public static string Invoke(string xFormerName, string dtoJson)
    {
        ITransformer xFormer = TransformerRegistry.Get(xFormerName);
        string xFromRes = xFormer.DtoToView(dtoJson);
        return xFromRes;
    }
}