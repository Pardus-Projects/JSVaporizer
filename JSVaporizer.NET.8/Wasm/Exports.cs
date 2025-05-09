﻿using System.Runtime.InteropServices.JavaScript;
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
        // ======================================================================== //
        //                  Event Listener and Generic Function pools               //
        // ======================================================================== //

        [JSExport]
        internal static int CallJSVEventListener(int id, JSObject elem, string eventType, JSObject evnt)
        {
            int behaviorMode = WasmJSVEventListenerPool.CallJSVEventListener(new EventListenerId(id), elem, eventType, evnt);
            return behaviorMode;
        }

        [JSExport]
        internal static void CallJSVGenericFunction(string funcKey, [JSMarshalAs<JSType.Array<JSType.Any>>] object[] args)
        {
             WasmJSVGenericFuncPool.CallJSVGenericFunction(funcKey, args);
        }

        // ======================================================================== //
        //                          DOM Mutation Observer                           //
        // ======================================================================== //

        // TODO
    }
}
