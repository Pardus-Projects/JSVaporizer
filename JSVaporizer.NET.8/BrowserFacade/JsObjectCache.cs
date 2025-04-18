using System;
using System.Collections.Concurrent;
using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;

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

        public static JSObject GetOrCreate(string elemId)
        {
            if (_cache.TryGetValue(elemId, out var wr) &&
                wr.TryGetTarget(out var alive) &&
                !alive.IsDisposed)
            {
                return alive;
            }

            // Proxy missing or disposed -> re‑hydrate from DOM
            var fresh = WasmDocument.GetElementById(elemId) ?? throw new JSVException($"Element id=\"{elemId}\" not found in DOM.");

            _cache[elemId] = new WeakReference<JSObject>(fresh);
            return fresh;
        }

        // Remove cache entry when Element is disposed.
        public static void Remove(string elemId) => _cache.TryRemove(elemId, out _);
    }
}
