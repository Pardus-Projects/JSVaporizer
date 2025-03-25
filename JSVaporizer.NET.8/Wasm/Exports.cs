using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;

namespace JSVaporizer;

// ============================================ //
//          Exports that JS can use             //
// ============================================ //

// See: https://learn.microsoft.com/en-us/aspnet/core/client-side/dotnet-interop/?view=aspnetcore-9.0

public static partial class JSVapor
{
    [SupportedOSPlatform("browser")]
    internal partial class WasmExports
    {
        [JSExport]
        internal static int CallJSVEventHandler(string funcKey, JSObject elem, string eventType, JSObject evnt)
        {
            int behaviorMode = WasmJSVEventHandlerPool.CallJSVEventHandler(funcKey, elem, eventType, evnt);
            return behaviorMode;
        }

        [JSExport]
        internal static void CallJSVGenericFunction(string funcKey, [JSMarshalAs<JSType.Array<JSType.Any>>] object[] args)
        {
             WasmJSVGenericFuncPool.CallJSVGenericFunction(funcKey, args);
        }

    }
}
