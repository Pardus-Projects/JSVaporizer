using JSVNuFlexiArch;
using NuFlexiArch;
using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;

namespace MyViewLib;

public partial class JSVComponentInitializer
{
    [JSExport]
    [SupportedOSPlatform("browser")]
    public static bool InstantiateFromJson(string instanceDtoJson, string referenceElementId)
    {
        return IJSVComponent.InstantiateFromJson(instanceDtoJson, referenceElementId);
    }
}