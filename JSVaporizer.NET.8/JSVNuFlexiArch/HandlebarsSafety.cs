using HandlebarsDotNet;
using System;
using System.Text.RegularExpressions;

namespace JSVNuFlexiArch;

public static class HandlebarsSafety
{
    private static readonly object _initLock = new();
    private static volatile bool _helpersRegistered = false;   // volatile so read isn't cached
    public static void RegisterHelpersOnce()
    {
        if (_helpersRegistered) return;

        lock (_initLock)
        {
            if (_helpersRegistered) return;      // someone else just did it while I was blocking

            // Allow raw when explicity required
            Handlebars.RegisterHelper("unescaped", (writer, context, parameters) =>
            {
                var raw = parameters[0]?.ToString() ?? "";
                writer.WriteSafeString(raw);
            });

            _helpersRegistered = true;
        }
    }

    // greedy = false (.*?) so we stop at the first closing "}}}"
    private static readonly Regex _rawPatternTripleBraces = new(@"{{{\s*?.*?}}}", RegexOptions.Singleline | RegexOptions.Compiled);
    public static HandlebarsTemplate<object, object> SafeCompile (string templateString)
    {
        RegisterHelpersOnce();

        // Look for triple braces, without the "unsafe" helper.
        var matchDefaultBad = _rawPatternTripleBraces.Match(templateString);
        if (matchDefaultBad.Success)
        {
            var offendingSnippet = matchDefaultBad.Value;
            string problem = $"Triple‑braces detected in \"{offendingSnippet}\". " +
                                "Raw output is not allowed by default; use {{unsafe Foo}} if you need it.";
            string exMessage = problem;
            exMessage += Environment.NewLine + "Full template:";
            exMessage += Environment.NewLine + templateString;
            exMessage += Environment.NewLine + problem; // Because of the way that exceptions are displayed when debugging.

            throw new HandlebarsException(exMessage);
        }

        try
        {
            HandlebarsTemplate<object, object> template = Handlebars.Compile(templateString);
            return template;
        }
        catch (Exception ex)
        {
            string exMessage = ex.ToString();
            exMessage += exMessage += Environment.NewLine + "Full template:";
            exMessage += Environment.NewLine + templateString;

            throw new HandlebarsException(exMessage);
        }
    }
}