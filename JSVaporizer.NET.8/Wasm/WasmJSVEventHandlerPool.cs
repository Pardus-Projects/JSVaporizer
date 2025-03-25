using System.Collections.Generic;
using System.Runtime.InteropServices.JavaScript;

namespace JSVaporizer;

// See: https://learn.microsoft.com/en-us/aspnet/core/client-side/dotnet-interop/?view=aspnetcore-9.0

// ============================================ //
//              Event Handlers                  //
// ============================================ //

public enum JSVEventHandlerBehavior {
    Default_Propagate,
    Default_NoPropagate,
    NoDefault_Propagate,
    NoDefault_NoPropagate
}

// Return value behaviorMode:
//      behaviorMode = 0 : preventDefault = false, stopPropagation = false
//      behaviorMode = 1 : preventDefault = false, stopPropagation = true
//      behaviorMode = 2 : preventDefault = true, stopPropagation = false
//      behaviorMode = 3 : preventDefault = true, stopPropagation = true
public delegate int EventHandlerCalledFromJS(JSObject elem, string eventType, JSObject evnt);

public static partial class JSVapor
{
    internal static class WasmJSVEventHandlerPool
    {
        private static readonly object _mutexJSVEventHandlerPool = new();
        private static Dictionary<string, EventHandlerCalledFromJS> _jsvEventHandlerPool = new();

        internal static void Add(string funcKey, EventHandlerCalledFromJS func)
        {
            if (_jsvEventHandlerPool.ContainsKey(funcKey))
            {
                throw new JSVException($"Key {funcKey} already exists. Remove first, and then add.");
            }

            // Use mutex. Is not thread safe.
            lock (_mutexJSVEventHandlerPool)
            {
                _jsvEventHandlerPool.TryAdd(funcKey, func);
            }
        }

        internal static bool Remove(string funcKey)
        {
            if (! _jsvEventHandlerPool.ContainsKey(funcKey))
            {
                throw new JSVException($"Key {funcKey} does not exist.");
            }

            // Use mutex. Is not thread safe.
            lock (_mutexJSVEventHandlerPool)
            {
                return _jsvEventHandlerPool.Remove(funcKey);
            }
        }

        internal static int CallJSVEventHandler(string funcKey, JSObject elem, string eventType, JSObject evnt)
        {
            return _jsvEventHandlerPool[funcKey](elem, eventType, evnt);
        }
    }
}



