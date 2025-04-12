using JSVZenView;
using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;

namespace ExampleViewLib;

public partial class MyComponentMaterializer
{
    [JSExport]
    [SupportedOSPlatform("browser")]
    public static bool MaterializeFromJson(string instanceDtoJson, string referenceElementId)
    {
        return ZenViewMaterializer.Materialize(instanceDtoJson, referenceElementId);
    }
}