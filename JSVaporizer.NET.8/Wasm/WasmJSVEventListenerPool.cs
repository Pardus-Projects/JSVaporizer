using System;
using System.Collections.Concurrent;
using System.Runtime.InteropServices.JavaScript;

namespace JSVaporizer;

// See: https://learn.microsoft.com/en-us/aspnet/core/client-side/dotnet-interop/?view=aspnetcore-9.0

// ============================================ //
//              Event Listeners                  //
// ============================================ //

public enum JSVEventListenerBehavior {
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
public delegate int EventListenerCalledFromJS(JSObject elem, string eventType, JSObject evnt);

public static partial class JSVapor
{
    internal static partial class WasmJSVEventListenerPool
    {
        private static readonly ConcurrentDictionary<EventListenerId, EventListenerCalledFromJS> _jsvEventListenerPool = new();
        private static int _nextId;

        public static EventListenerId Add(EventListenerCalledFromJS listener)
        {
            var id = new EventListenerId(System.Threading.Interlocked.Increment(ref _nextId));
            _jsvEventListenerPool[id] = listener;
#if DEBUG
            EventListenerDebugInfo.Record(id);
            //Window.Alert("ADD: " + id + ", current count = " + EventListenerDebugInfo.MapKeyCount());
#endif
            return id;
        }

        public static bool Remove(EventListenerId id)
        {
            var ok = _jsvEventListenerPool.TryRemove(id, out _);
#if DEBUG
            EventListenerDebugInfo.Remove(id);
            //Window.Alert("REMOVE: " + id + ", current count = " + EventListenerDebugInfo.MapKeyCount());
#endif
            return ok;
        }

        public static int CallJSVEventListener(EventListenerId id, JSObject elem, string type, JSObject evnt)
        {
            bool gotIt = _jsvEventListenerPool.TryGetValue(id, out var del);
            if (!gotIt)
            {
                throw new ArgumentException($"EventListenerId with id = {id} is not present in _jsvEventListenerPool");
            }
            return del!(elem, type, evnt);
        }
    }
}

