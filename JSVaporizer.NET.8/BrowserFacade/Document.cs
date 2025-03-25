using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;

namespace JSVaporizer;

// https://developer.mozilla.org/en-US/docs/Web/API/Document

public static partial class JSVapor
{
    [SupportedOSPlatform("browser")]
    public static bool DisposeIfConnectedToDOM(JSObject jSObject)
    {
        if (jSObject.GetPropertyAsBoolean("isConnected"))
        {
            jSObject.Dispose();
            return true;
        }
        else
        {
            return false;
        }
    }

    [SupportedOSPlatform("browser")]
    public static class Document
    {
        // This is used to detect whether a DOM element was created using JSVaporizer.
        //
        // This must be lower case, since in JS element.setAttribute() converts the attribute name to lower case.
        private static string _createdByJSVaporizerAttributeName = "created-by-jsvaporizer";

        // This is used for bookkeeping.
        //
        // The dictionary key is then element Id.
        //
        // In order to wrap up actual DOM elements inside our Element facade classes,
        //  we need to keep track of which ones are lying around,
        //  since you can't find them in the actual DOM when they are orphans (not connected).
        //
        // This needs to be updated upon every creation and destruction of an Element.
        // In addition, we can use the custom attribute
        //      _createdByJSVaporizerAttributeName
        // to perform reconciliation.
        private static Dictionary<string, Element> _jsvElements = new();

        // Props

        // ---------------------------------------------------------------------- //
        // ----- Document standard ---------------------------------------------- //
        // ---------------------------------------------------------------------- //

        //public static List<Element> Children { get; } = new();

        // Methods

        // ---------------------------------------------------------------------- //
        // ----- JSVaporizer ---------------------------------------------------- //
        // ---------------------------------------------------------------------- //

        public static string CreatedByJSV { get { return _createdByJSVaporizerAttributeName; } }

        public static Element AssertGetElementById(string id)
        {
            Element? elem = GetElementById(id);
            if (elem == null)
            {
                throw new JSVException($"Element with id={id} not found.");
            }
            return elem;
        }

        // ---------------------------------------------------------------------- //
        // ----- Standard ------------------------------------------------------- //
        // ---------------------------------------------------------------------- //

        public static Element CreateElement(string id, string tagName)
        {
            // We absolutely want this to throw if the key is already there.
            // Id values need to be unique.

            if (_jsvElements.ContainsKey(id))
            {
                throw new JSVException($"A element with id={id} already exists.");
            }

            JSObject? jSObject = null;
            try
            {
                jSObject = JSVapor.WasmDocument.CreateJSVaporizerElement(id, tagName, _createdByJSVaporizerAttributeName);

                Element elem = new Element(id, jSObject);

                // Now set the actual value in _jsvElements.
                _jsvElements[id] = elem;

                return elem;
            }
            finally
            {
                if (jSObject != null) DisposeIfConnectedToDOM(jSObject);
            }
        }

        public static Element? GetElementById(string id)
        {
            // First see if it's one that JSVaporizer made.
            if (_jsvElements.ContainsKey(id))
            {
                return _jsvElements[id];
            }
            // Otherwise return whatever the DOM says we have.
            else
            {
                JSObject? jSObject = null;
                try
                {
                    jSObject = JSVapor.WasmDocument.GetElementById(id);
                    if (jSObject != null)
                    {
                        return new Element(id);
                    }
                    else
                    {
                        return null;
                    }
                }
                finally
                {
                    if (jSObject != null) DisposeIfConnectedToDOM(jSObject);
                }
            }
        }

        public static List<JSObject> GetElementsByTagName(string tagName)
        {
            // https://learn.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.javascript.jsobject?view=net-9.0
            //      Don't carry around a JSObject for each element.
            //      They are expensive, so we will look them up on the fly
            //      then use JSObject.Dispose() to dispose them.

            JSObject[] jSObjectArr = JSVapor.WasmDocument.GetElementsArrayByTagName(tagName);
            return jSObjectArr.ToList();
        }

        //public static Element? QuerySelector(string selectors)
        //{
        //    throw new NotImplementedException();
        //}

        //publicstatic  List<Element> QuerySelectorAll(string selectors)
        //{
        //    throw new NotImplementedException();
        //}

        //public static void ReplaceChildren(List<Element> newChildren)
        //{
        //    throw new NotImplementedException();
        //}

        // ---------------------------------------------------------------------- //
        // ----- JSVaporizer ---------------------------------------------------- //
        // ---------------------------------------------------------------------- //

        // Not sure exactly when/how we're going to need this,
        // but we'll probably need some version of this for something.
        public static string JSVToDomReconciliation()
        {
            // Find all JSObjects connected to DOM.
            List<JSObject> jSObjectList = GetElementsByTagName("*");

            // Counts of DOM elements which are JSV, grouped by groupKey.
            Dictionary<string, int> domJSVCounts = new();

            // Counts of DOM elements which are JSV, grouped by groupKey.
            Dictionary<string, int> domNotJSVCounts = new();

            foreach (JSObject jSObject in jSObjectList)
            {
                string id = WasmElement.GetAttribute(jSObject, "id") ?? "<NO_ID>";
                string tagName = jSObject.GetPropertyAsString("tagName") ?? "<NO_TAG>";

                if (WasmElement.HasAttribute(jSObject, CreatedByJSV))
                {
                    string groupKey = $"{id}";                  // Not the same key as for domNotJSVCounts

                    if (!domJSVCounts.ContainsKey(groupKey))
                    {
                        domJSVCounts[groupKey] = 0;
                    }
                    domJSVCounts[groupKey]++;
                }
                else
                {
                    string groupKey = $"{id} : {tagName}";      // Not the same key as for domJSVCounts

                    if (!domNotJSVCounts.ContainsKey(groupKey))
                    {
                        domNotJSVCounts[groupKey] = 0;
                    }
                    domNotJSVCounts[groupKey]++;
                }

                // Dispose JSObjects;
                DisposeIfConnectedToDOM(jSObject);
            }

            // Reconcile

            HashSet<string> jsvIds = new(_jsvElements.Keys);
            HashSet<string> domJSVIds = new(domJSVCounts.Keys);

            HashSet<string> jsvNoDom = new(jsvIds);
            jsvNoDom.ExceptWith(domJSVIds);

            HashSet<string> domNoJsv = new(domJSVIds);
            domNoJsv.ExceptWith(jsvIds);

            string problems = "";
            if (jsvNoDom.Count > 0)
            {
                problems += "The following JSV ids were found in _jsvElement but not the DOM:";
                problems += Environment.NewLine;
                problems += string.Join("," + Environment.NewLine, jsvNoDom.ToList());
            }
            if (domNoJsv.Count > 0)
            {
                problems += Environment.NewLine;
                problems += "The following JSV ids were found in the DOM but not in _jsvElements:";
                problems += Environment.NewLine;
                problems += string.Join("," + Environment.NewLine, domNoJsv.ToList()) + Environment.NewLine;
            }

            return problems;
        }
    }
}
