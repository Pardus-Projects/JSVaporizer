# JSVaporizer

JSVaporizer is a minimal, opinionated .NET-to-JavaScript interop layer for WebAssembly-based apps. It provides two-way bindings:

1. **.NET → JavaScript** through `[JSImport]` wrappers.
2. **JavaScript → .NET** via `[JSExport]` methods and delegate pools for event handling and generic function invocation.

The code in this repository allows you to:
- Dynamically create and manage DOM elements from the .NET side.
- Call existing JavaScript functions (and function properties) by name.
- Register C# delegates so that JavaScript can call into them (useful for event handlers, utilities, etc.).
- Maintain dictionaries of these delegates (function pools), avoiding direct references in JS while still enabling dynamic binding.

---

## Table of Contents

1. [Requirements](#requirements)  
2. [Key Concepts](#key-concepts)  
3. [Examples](#examples)  
   1. [Creating a DOM Element in .NET](#creating-a-dom-element-in-net)  
   2. [Calling JavaScript Functions from .NET](#calling-javascript-functions-from-net)  
   3. [Adding Event Handlers](#adding-event-handlers)  
   4. [Registering a Generic Function for JS to Call](#registering-a-generic-function-for-js-to-call)  
4. [Project Structure](#project-structure)  
5. [Thread Safety](#thread-safety)  
6. [Performance Considerations](#performance-considerations)  
7. [License](#license)  

---

### Project Structure

```csharp
JSVaporizer/
|
├── JSVapor.cs
|    ├── public static partial class JSVapor
|    │     ├── internal partial class WasmExports        // [JSExport] methods
|    │     ├── internal partial class WasmElement        // [JSImport] "element"...
|    │     ├── internal partial class WasmDocument       // [JSImport] "document"...
|    │     ├── internal partial class WasmWindow         // [JSImport] "window"...
|    │     ├── internal partial class WasmJSFunctionPool // [JSImport] "jsFunctionPool"...
|    │     ├── internal partial class WasmJSVEventHandlerPool
|    │     ├── internal partial class WasmJSVGenericFuncPool
|    │     └── (etc.)
|
├── Element.cs    // The JSVapor.Element class, a higher-level wrapper for DOM elements
├── Document.cs   // The JSVapor.Document class, includes the dictionary of known Elements, creation, getById, etc.
├── Other .cs files for exceptions, shared enumerations, etc.
|
└── README.md

```

## Requirements

- .NET 7 or higher.  
- WebAssembly-compatible runtime (e.g., Blazor WebAssembly).  
- A build or project setup that supports C#-to-JS interop using `[JSExport]` and `[JSImport]` (e.g., an ASP.NET Core WebAssembly project referencing `System.Runtime.InteropServices.JavaScript`).  

---

## Key Concepts

### 1. Partial Classes for Interop

All JS-bound methods are grouped into nested **partial** classes under a common `JSVapor` static class. For example, `WasmElement`, `WasmDocument`, and `WasmWindow` are partial classes that each contain `[JSImport]` methods to call JavaScript functions.

Similarly, `WasmExports` is where we place `[JSExport]` methods—these are .NET methods callable from JavaScript.

### 2. Event Handler and Generic Function Pools

We keep dictionaries to store .NET delegates:
- **`WasmJSVEventHandlerPool`**: Holds delegates for DOM event handlers.  
- **`WasmJSVGenericFuncPool`**: Holds delegates for arbitrary functions that JavaScript can invoke by key.  

This approach avoids creating direct named methods for every single handler in `[JSExport]`. Instead, JavaScript only needs a “function key,” and we look up the actual C# method in the dictionary.

### 3. Ephemeral `JSObject` References

When you request a DOM `JSObject` for an element, the code automatically disposes it if it’s connected (`isConnected == true`). This ensures we don’t hold on to many `JSObject` instances in .NET, which can hurt memory usage. On subsequent calls, the system retrieves a fresh reference by calling `document.getElementById` or similar.

### 4. Custom Exceptions and Error Handling

We use a custom `JSVException` class for domain-specific errors (e.g., attribute name is not lowercase, key collisions, missing event handlers). This is to keep interop errors well-defined rather than throwing generic exceptions.

---

## Examples

### Creating a DOM Element in .NET

```csharp
// Create a new <div> element with id="myDiv"
var newDiv = JSVapor.Document.CreateElement("myDiv", "div");

// Set an attribute
newDiv.SetAttribute("class", "myClass");

// Append it to another element
var parentElement = JSVapor.Document.AssertGetElementById("parentContainer");
parentElement.AppendChild(newDiv);

```

### Calling JavaScript Functions from .NET

```csharp
// Using the WasmJSFunctionPool (which calls JS by function key)
var result = JSVapor.JSFunctionPool.CallFunc("myJsFunctionKey", new object[] { "arg1", 42 });
```

Here, "myJsFunctionKey" corresponds to a JavaScript-side registration (for example, your JS might have something like window.jsFunctionPool.myJsFunctionKey = function(a, b) { ... }).

### Adding Event Handlers

```csharp
// Suppose you want to handle a "click" event on your newly created div.
newDiv.AddEventListener(
    "click", 
    funcKey: "myClickHandler",
    handler: (elem, eventType, evnt) =>
    {
        // do your .NET side logic
        // return an int that indicates whether to stopPropagation or preventDefault
        return 0; // e.g., 0 = default prop, 1 = default no prop, etc.
    }
);
```

When JavaScript triggers the click event, it calls into .NET by using the `[JSExport]` method `CallJSVEventHandler`, which looks up your handler in `WasmJSVEventHandlerPool`.

### Registering a Generic Function for JS to Call

```csharp
// In .NET, define a function you want JavaScript to call:
JSVGenericFunction myFunc = (object[] args) =>
{
    // Implementation here...
    return "Hello from .NET!";
};

// Register it by key
JSVapor.JSVGenericFunctionPool.RegisterJSVGenericFunction("myGenericFuncKey", myFunc);

// On the JavaScript side, you'd do something like:
//   const result = Module.jsvGenericFunctionPool.callJSVGenericFunction("myGenericFuncKey", ["any", "args"]);
```

