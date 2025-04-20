// Interop/EventListenerId.cs — NEW FILE
using System;
using System.Collections.Concurrent;

namespace JSVaporizer;

// Fast, opaque handle for an attached .NET listener.
public readonly record struct EventListenerId(int Value)
{
    public override string ToString() => Value.ToString();
}

// Interop/EventListenerDebugInfo.cs — NEW FILE (DEBUG‑only)
#if DEBUG

// Holds creation stack‑traces for listener ids (debug builds only).
public static class EventListenerDebugInfo
{
    private static readonly ConcurrentDictionary<EventListenerId, string> _map = new();

    public static void Record(EventListenerId id)
        => _map[id] = Environment.StackTrace;

    public static string? GetTrace(EventListenerId id)
        => _map.TryGetValue(id, out var t) ? t : null;

    public static void Remove(EventListenerId id)
        => _map.TryRemove(id, out _);

    public static string Dump()
    {
        string dump = "";
        int counter = 0;
        foreach (EventListenerId id in _map.Keys)
        {
            string stackTrace = _map[id];
            counter += 1;
            dump += Environment.NewLine + counter + " : " + id;
            dump += Environment.NewLine + "---";
            dump += Environment.NewLine + stackTrace;
            dump += Environment.NewLine + "================================================";
            dump += Environment.NewLine;
        }

        return dump;
    }

    public static int MapKeyCount()
    {
        return _map.Keys.Count;
    }
}
#endif

