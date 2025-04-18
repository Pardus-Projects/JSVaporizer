using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;

namespace JSVaporizer;

// https://developer.mozilla.org/en-US/docs/Web/API/Element

public static partial class JSVapor
{
    [SupportedOSPlatform("browser")]
    public class Element : IDisposable
    {
        private bool _isDisposed;

        // https://learn.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.javascript.jsobject?view=net-9.0
        //      JSObject objects are expensive.
        //      We should  carry around a JSObject only before the element is added to the DOM.
        //
        // This will be set to null when the Element is connected to the DOM.
        private JSObject? _ephemeralJSObject = null;

        // Keep track of eveant listeners assigned to element.
        private readonly Dictionary<string, HashSet<EventHandlerToken>> _eventListenersByType = new();

        // Props

        // ---------------------------------------------------------------------- //
        // ----- JSVaporizer ---------------------------------------------------- //
        // ---------------------------------------------------------------------- //

        public bool IsJSV { get { return HasAttribute(Document.CreatedByJSV); } }

        // ---------------------------------------------------------------------- //
        // ----- Element standard ----------------------------------------------- //
        // ---------------------------------------------------------------------- //

        public string Id { get; }

        // Ctor

        public Element(string id, JSObject? jSObject = null)
        {
            _ephemeralJSObject = jSObject;
            Id = id;
        }

        // Methods

        // ---------------------------------------------------------------------- //
        // ----- JSVaporizer ---------------------------------------------------- //
        // ---------------------------------------------------------------------- //

        //      NOTE: The following come built into JSObject:
        //          1) JSObject.SetProperty()
        //          2) JSObject.HasProperty()
        //          3) JSObject.GetPropertyAsBoolean()
        //          4) JSObject.GetPropertyAsByteArray()
        //          5) JSObject.GetPropertyAsDouble()
        //          6) JSObject.GetPropertyAsInt32()
        //          7) JSObject.GetPropertyAsJSObject()
        //          8) JSObject.GetPropertyAsString()
        //      SEE: https://learn.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.javascript.jsobject?view=net-9.0

        public JSObject GetJSObject()
        {
            if (_isDisposed)
                throw new ObjectDisposedException($"Element {Id} has been disposed.");

            // Null out _ephemeralJSObject if it is disposed.
            // This can happen any time after the element is connected to the DOM.
            if (_ephemeralJSObject != null && _ephemeralJSObject.IsDisposed)
            {
                _ephemeralJSObject = null;
            }

            if (_ephemeralJSObject != null)
            {
                return _ephemeralJSObject;
            }


            // node should now be in DOM – use central cache
            return JsObjectCache.GetOrCreate(Id);
        }

        public void AssertProperty(string propName)
        {
            bool hasProp = HasProperty(propName);
            if (!hasProp)
            {
                throw new JSVException($"element with id={Id} does not have the \"{propName}\" property.");
            }
        }

        public bool HasProperty(string propName) {
            return GetJSObject().HasProperty(propName);
        }

        public void SetProperty(string propName, object propVal)
        {
            if (propName == "id")
            {
                throw new JSVException("FIXME: You shouldn't set \"id\" this way until bookkeeping is improved to handle it correctly.");
            }

            JSObject jSObject = GetJSObject();

            if (propVal is bool)
            {
                jSObject.SetProperty(propName, (bool)propVal);
            }
            else if (propVal is byte[])
            {
                jSObject.SetProperty(propName, (byte[])propVal);
            }
            else if (propVal is double)
            {
                jSObject.SetProperty(propName, (double)propVal);
            }
            else if (propVal is int)
            {
                jSObject.SetProperty(propName, (int)propVal);
            }
            else if (propVal is string)
            {
                jSObject.SetProperty(propName, (string)propVal);
            }
            else if (propVal is JSObject)
            {
                jSObject.SetProperty(propName, (JSObject)propVal);
            }
            else
            {
                throw new JSVException($"propVal has type {propVal.GetType().ToString()}, which is not allowed here.");
            }
        }

        public ElementPropInfo GetProperty(string propName)
        {
            JSObject jSObject = GetJSObject();

            if (!jSObject.HasProperty(propName))
            {
                throw new JSVException($"Property \"{propName}\" does not exist.");
            }

            // See: https://learn.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.javascript.jsobject.gettypeofproperty?view=net-9.0#system-runtime-interopservices-javascript-jsobject-gettypeofproperty(system-string)
            //  Property types are:
            //      "undefined"     we handle this
            //      "object"        we handle this
            //      "boolean"       we handle this
            //      "number"        we handle this
            //      "string"        we handle this
            //      "bigint"        NOT HANDLED YET
            //      "symbol"        NOT HANDLED YET
            //      "function"      NOT HANDLED YET

            string propType = jSObject.GetTypeOfProperty(propName);

            ElementPropInfo propInfo;

            if (propType == "object")
            {
                propInfo = new(propName, propType, jSObject.GetPropertyAsJSObject(propName), false);
            }
            else if (propType == "boolean")
            {
                propInfo = new(propName, propType, jSObject.GetPropertyAsBoolean(propName), false);
            }
            else if (propType == "number")
            {
                propInfo = new(propName, propType, jSObject.GetPropertyAsDouble(propName), false);
            }
            else if (propType == "string")
            {
                    propInfo = new(propName, propType, jSObject.GetPropertyAsString(propName), false);
                }
            else if (propType == "function")
            {
                propInfo = new(propName, propType, null, true);
            }
            else // bigint, symbol
            {
                propInfo = new(propName, propType, null, true);
            }

            return propInfo;
        }

        public List<string> GetPropertyNamesList()
        {
            JSObject jSObject = GetJSObject();
            List<string> propNames = WasmElement.GetPropertyNamesArray(jSObject).ToList();

            return propNames;
        }

        public Dictionary<string, ElementPropInfo> GetPropertiesDictionary()
        {
            List<string> propNameList = GetPropertyNamesList();

            Dictionary<string, ElementPropInfo> propsDict = new();
            foreach (string propName in propNameList)
            {
                ElementPropInfo propInfo = GetProperty(propName);
                propsDict[propName] = propInfo;
            }

            return propsDict;
        }

        public void InvokeFuncProp(string funcPropName, object[]? args = null)
        {
            // Make sure this is a valid thing to try.
            ElementPropInfo propInfo = GetProperty(funcPropName);
            if (propInfo.Type != "function")
            {
                throw new JSVException($"Property \"{funcPropName}\" is not a function.");
            }

            if (args == null)
            {
                args = [];
            }

            JSObject jSObject = GetJSObject();
            WasmElement.InvokeFunctionProperty(jSObject, funcPropName, args);
        }

        public List<string> GetMultiSelectOptionValues()
        {
            JSObject jSObject = GetJSObject();
            return WasmElement.GetMultiSelectOptionValues(jSObject).ToList();
        }


        // ---------------------------------------------------------------------- //
        // ----- Standard ------------------------------------------------------- //
        // ---------------------------------------------------------------------- //

        public Element AppendChild(Element childElem)
        {
            JSObject parentJSObject = GetJSObject();
            JSObject childJSObject = childElem.GetJSObject();
            WasmElement.AppendChild(parentJSObject, childJSObject);

            if (childJSObject != null && childJSObject.GetPropertyAsBoolean("isConnected"))
            {
                childJSObject.Dispose();
                childElem._ephemeralJSObject = null;
            }

            return childElem;
        }

        public bool HasAttribute(string attrName)
        {
            if (attrName != attrName.ToLower())
            {
                throw new JSVException($"attrName=\"{attrName}\" will not work because it is not lower case. JS converts them to lower case.");
            }

            JSObject jSObject = GetJSObject();
            bool hasAttr = WasmElement.HasAttribute(jSObject, attrName);

            return hasAttr;
        }

        public string? GetAttribute(string attrName)
        {
            if (attrName != attrName.ToLower())
            {
                throw new JSVException($"attrName=\"{attrName}\" will not work because it is not lower case. JS converts them to lower case.");
            }

            JSObject jSObject = GetJSObject();
            string? attrVal = WasmElement.GetAttribute(jSObject, attrName);

            return attrVal;
        }

        public void SetAttribute(string attrName, string attrValue)
        {
            if (attrName != attrName.ToLower())
            {
                throw new JSVException($"attrName=\"{attrName}\" will not work because it is not lower case. JS converts them to lower case.");
            }

            if (attrName == "id")
            {
                throw new JSVException("FIXME: You shouldn't set \"id\" this way until bookkeeping is improved to handle it correctly.");
            }

            JSObject jSObject = GetJSObject();
            WasmElement.SetAttribute(jSObject, attrName, attrValue); 
        }

        public IDisposable AddEventListener(string eventType, EventListenerCalledFromJS handler)
        {
            EventListenerId id = WasmJSVEventListenerPool.Add(handler);
            EventHandlerToken token = new EventHandlerToken(this, eventType, id);

            using var js = GetJSObject();
            int listenerCountFromJS = WasmElement.AddEventListener(js, eventType, id.Value);

            //Window.Alert("AddEventListener: " + listenerCountFromJS);

            if (!_eventListenersByType.TryGetValue(eventType, out var set))
                _eventListenersByType[eventType] = set = new();
            set.Add(token);

            return token; // caller can ignore or Dispose later
        }

        private void RemoveEventListener(EventListenerId id, string eventType)
        {
            using var js = GetJSObject();
            int listenerCountFromJS = WasmElement.RemoveEventListener(js, eventType, id.Value);
            WasmJSVEventListenerPool.Remove(id);

            //Window.Alert("RemoveEventListener: " + listenerCountFromJS);

            if (_eventListenersByType.TryGetValue(eventType, out var set))
            {
                set.RemoveWhere(t => t.Id.Equals(id));
            }
        }

        public void Dispose()
        {
            if (_isDisposed) return;

            foreach (var set in _eventListenersByType.Values)
            {
                foreach (var tok in set.ToArray())
                {
                    tok.Dispose();
                }
            }
            _eventListenersByType.Clear();

            JsObjectCache.Remove(Id);
            _isDisposed = true;
        }

        // Returned by AddEventListener().
        // Call Dispose() to detach the DOM handler.
        private sealed record EventHandlerToken(Element Owner, string EventType, EventListenerId Id) : IDisposable
        {
            public void Dispose()
            {
                Owner.RemoveEventListener(Id, EventType);
            }
        }
    }

    public class ElementPropInfo
    {
        public readonly string Name;
        public readonly string Type;
        public readonly bool NotHandled;
        public readonly object? Value;

        public ElementPropInfo(string name, string type, object? value, bool notHandled)
        {
            Name = name;
            Type = type;
            Value = value;
            NotHandled = notHandled;
        }
    }
}
