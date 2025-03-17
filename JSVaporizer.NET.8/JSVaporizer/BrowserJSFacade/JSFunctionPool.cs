using System.Runtime.Versioning;

namespace JSVaporizer;

public static partial class JSVapor
{
    [SupportedOSPlatform("browser")]
    public static class JSFunctionPool
    {
        public static object? CallFunc(string funcKey, object[] args)
        {
            return WasmJSFunctionPool.CallJSFunction(funcKey, args);
        }
    }
}


