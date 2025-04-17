using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;

namespace JSVNuFlexiArch;

[SupportedOSPlatform("browser")]
public static partial class JSVCompBuilderInvoker
{
    [JSExport]
    public static void Invoke(string uniqueName, string assemblyQualifiedName, string placeholderElementId)
    {
        JSVCompBuilder.Invoke(uniqueName, assemblyQualifiedName, placeholderElementId);
    }
}

[SupportedOSPlatform("browser")]
public static partial class JSVTransformerInvoker
{
    [JSExport]
    [SupportedOSPlatform("browser")]
    public static string Invoke(string assemblyQualifiedName, string dtoJson, string? userInfoJson = null)
    {
        return JSVTransformer.Invoke(assemblyQualifiedName, dtoJson, userInfoJson);
    }
}
