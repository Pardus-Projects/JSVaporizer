using Ganss.Xss;

namespace JSVaporizer;

public static partial class JSVapor
{
    public static class HtmlSafety
    {
        private static readonly HtmlSanitizer _san = new();      // configure once
        public static string Safe(string? html) => _san.Sanitize(html ?? "");
    }
}
