using System;
using System.Diagnostics;

namespace JSVaporizer;

// Because we want to be able to see the stack trace in the browser.
public static partial class JSVapor
{
    private static string StackTrace(int framesToRemove = 1, bool lineNumbers = true)
    {
        StackTrace st = new StackTrace(framesToRemove, lineNumbers);
        return st.ToString();
    }

    public class JSVException : Exception
    {
        string _overrideStackTrace;
        public override string? StackTrace { get { return _overrideStackTrace; } }

        public JSVException(string message) : base(message + Environment.NewLine + StackTrace(2))
        {
            _overrideStackTrace = StackTrace(2);
        }
    }
}


