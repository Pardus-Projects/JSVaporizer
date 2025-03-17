using System.Collections.Generic;
using System.Runtime.InteropServices.JavaScript;

namespace JSVaporizer;

// See: https://learn.microsoft.com/en-us/aspnet/core/client-side/dotnet-interop/?view=aspnetcore-9.0

// ============================================ //
//              JSV Function Pool               //
// ============================================ //

public delegate object? JSVGenericFunction(object[] args);

public static partial class JSVapor
{
    private protected static partial class WasmJSVGenericFuncPool
    {
        private static readonly object _mutexJSVFunctionPool = new();
        private static Dictionary<string, JSVGenericFunction> _jsvFunctionPool = new();

        internal static void Add(string funcKey, JSVGenericFunction func)
        {
            if (_jsvFunctionPool.ContainsKey(funcKey))
            {
                throw new JSVException($"Key {funcKey} already exists. Remove first, and then add.");
            }

            // Use mutex. Is not thread safe.
            lock (_mutexJSVFunctionPool)
            {
                _jsvFunctionPool.TryAdd(funcKey, func);
            }
        }

        internal static bool Remove(string funcKey)
        {
            if (!_jsvFunctionPool.ContainsKey(funcKey))
            {
                throw new JSVException($"Key {funcKey} does not exist.");
            }

            // Use mutex. Is not thread safe.
            lock (_mutexJSVFunctionPool)
            {
                return _jsvFunctionPool.Remove(funcKey);
            }
        }

        internal static object? CallJSVGenericFunction(string funcKey, object[] args)
        {
            return _jsvFunctionPool[funcKey](args);
        }
    }
}



