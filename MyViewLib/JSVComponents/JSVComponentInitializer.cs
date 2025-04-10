using JSVNuFlexiArch;
using JSVNuFlexiArch;
using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;

namespace MyViewLib;

public partial class JSVComponentInitializer
{
    [JSExport]
    [SupportedOSPlatform("browser")]
    public static bool InstantiateAndRenderFromJson(string instanceDtoJson, string referenceElementId)
    {
        return JSVComponentMaterializer.InstantiateAndRenderFromJson(instanceDtoJson, referenceElementId);
    }
}