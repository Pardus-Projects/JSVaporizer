using System;
using System.Collections.Concurrent;

namespace JSVaporizer;

// See: https://learn.microsoft.com/en-us/aspnet/core/client-side/dotnet-interop/?view=aspnetcore-9.0

// ============================================ //
//              JSV Function Pool               //
// ============================================ //

public delegate object? JSVGenericFunction(object[] args);

public static partial class JSVapor
{
    internal static partial class WasmJSVGenericFuncPool
    {
        private static readonly ConcurrentDictionary<string, JSVGenericFunction> _jsvFunctionPool = new(StringComparer.Ordinal);

        internal static void Add(string funcKey, JSVGenericFunction func)
        {
            if (!_jsvFunctionPool.TryAdd(funcKey, func))
            {
                throw new JSVException($"Key \"{funcKey}\" already registered.");
            }   
        }

        internal static void Remove(string funcKey)
        {
            bool removed =  _jsvFunctionPool.TryRemove(funcKey, out _);
            if (!removed)
            {
                throw new JSVException($"Key \"{funcKey}\" is not present.");
            }
        }

        internal static object? CallJSVGenericFunction(string funcKey, object[] args)
        {
            if (_jsvFunctionPool.TryGetValue(funcKey, out var del))
            {
                return del(args);
            }    

            throw new JSVException($"Key \"{funcKey}\" is not present.");
        }
    }
}