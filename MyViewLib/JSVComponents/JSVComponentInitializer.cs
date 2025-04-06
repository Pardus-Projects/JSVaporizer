using JSVNuFlexiArch;
using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;

namespace MyViewLib;

public partial class JSVComponentInitializer
{
    [JSExport]
    [SupportedOSPlatform("browser")]
    public static bool InitializeFromJson(string metadataJson, string stateDtoJson)
    {
        return IJSVComponent.InitializeFromJson(metadataJson, stateDtoJson);
    }
}