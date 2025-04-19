using System.Runtime.Versioning;

namespace JSVaporizer;

public static partial class JSVapor
{
    [SupportedOSPlatform("browser")]
    public static class Window
    {

#if DEBUG
        // window.alert halts the JS event‑loop, breaks keyboard navigation, fails WCAG 2.2, and annoys users.
        public static string Alert(object? obj)
        {
            string text = obj?.ToString() ?? "";
            return WasmWindow.Alert(text);
        }
#endif

        public static class Location
        {
            public static string Href()
            {
                return WasmWindow.Location.Href();
            }
        }
    }

    [SupportedOSPlatform("browser")]
    public static class Console
    {
        public static void Log(string str)
        {
            WasmWindow.Console.Log(str);
        }

        public static void Dir(object obj)
        {
            WasmWindow.Console.Dir(obj);
        }
    }
}


