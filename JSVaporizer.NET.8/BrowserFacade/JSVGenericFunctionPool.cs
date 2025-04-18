using System.Runtime.Versioning;

namespace JSVaporizer;

public static partial class JSVapor
{
    [SupportedOSPlatform("browser")]
    public static class JSVGenericFunctionPool
    {
        public static void RegisterJSVGenericFunction(string funcKey, JSVGenericFunction func)
        {
            WasmJSVGenericFuncPool.Add(funcKey, func);
        }

        public static void UnregisterJSVGenericFunction(string funcKey)
        {
            WasmJSVGenericFuncPool.Remove(funcKey);
        }
    }
}


