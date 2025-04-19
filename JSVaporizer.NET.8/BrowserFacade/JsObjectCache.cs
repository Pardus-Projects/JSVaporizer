using System;
using System.Collections.Concurrent;
using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;
using System.Threading;

namespace JSVaporizer;

public static partial class JSVapor
{
    // Central store for live JSObject proxies, keyed by DOM element id.
    // Guarantees: one proxy per id; re‑hydrates if the old proxy was disposed.
    [SupportedOSPlatform("browser")]
    internal static class JsObjectCache
    {
        // WeakReference so GC can reclaim JSObject when browser drops it.
        private static readonly ConcurrentDictionary<string, WeakReference<JSObject>> _cache = new();

        // Sweep  once every N calls
        private const int SweepInterval = 100;
        private static int _opCounter;

        public static JSObject GetOrCreate(string elemId)
        {
            // Opportunistic sweep
            if (Interlocked.Increment(ref _opCounter) >= SweepInterval)
            {
                Sweep();
                Interlocked.Exchange(ref _opCounter, 0);
            }

            if (_cache.TryGetValue(elemId, out var weakRef)
                && weakRef.TryGetTarget(out var jsObject)
                && !jsObject.IsDisposed
                && IsConnected(jsObject))
            {
                return jsObject;
            }

            // Proxy missing or disposed -> re‑hydrate from DOM
            var fresh = WasmDocument.GetElementById(elemId) ?? throw new JSVException($"Element id=\"{elemId}\" not found in DOM.");

            _cache[elemId] = new WeakReference<JSObject>(fresh);
            return fresh;
        }

        // Remove cache entry when Element is disposed.
        public static void Remove(string elemId) => _cache.TryRemove(elemId, out _);

        private static void Sweep()
        {
            foreach (var kvp in _cache)
            {
                var weakRef = kvp.Value;
                if (!weakRef.TryGetTarget(out var jsObject) || jsObject.IsDisposed || !IsConnected(jsObject))
                {
                    _cache.TryRemove(kvp.Key, out _);
                }         
            }
        }

        private static bool IsConnected(JSObject js)
        {
            try
            {
                return js.GetPropertyAsBoolean("isConnected");
            }
            catch
            {
                return false; // property missing or JSObject already gone
            }
        }
    }
}
