# JSVaporizer & JSVNuFlexiArch

A **lightweight, incremental** approach to adding WebAssembly-driven, C#-based client functionality to existing Razor Pages or MVC applications—without rewriting everything in Blazor or duplicating logic in JavaScript.

## Table of Contents

1. [Motivation](#motivation)  
2. [Key Features](#key-features)  
3. [How It Works](#how-it-works)  
   1. [C# Interop Classes (JSVaporizer)](#c-interop-classes-jsvaporizer)  
   2. [UI Components (JSVNuFlexiArch)](#ui-components-jsvnuflexiarch)  
   3. [JavaScript Modules](#javascript-modules)  
4. [Usage](#usage)  
   1. [Project Setup](#project-setup)  
   2. [Initialization](#initialization)  
   3. [Creating and Using Components](#creating-and-using-components)  
   4. [Event Handling](#event-handling)  
5. [Advantages](#advantages)  
6. [Limitations and Why Not to Use](#limitations-and-why-not-to-use)  
7. [Examples](#examples)  
8. [License](#license)  
9. [Contributing](#contributing)

---

## Motivation

Many applications use **Razor Pages** or **MVC** for server-side rendering. Over time, front-end JavaScript can become unmaintainable “spaghetti” — particularly if you’re duplicating **DTO and validation logic** in both C# and JS.

Some teams would like to adopt **WebAssembly** for dynamic behaviors in the browser, but **migrating fully** to Blazor or a large SPA framework can be **overkill or infeasible**. This toolkit was created to:

- **Avoid duplicated logic**: Keep validation, data transfer objects, and core logic in shared C#.
- **Incrementally introduce client-side interactivity**: Without rewriting your entire Razor Pages app.
- **Maintain full control**: Provide typed access to the DOM and events, rather than hiding them behind heavy abstractions.
- **Keep it small**: No built-in routing or large ecosystem — just a straightforward bridge between .NET and the browser DOM.

---

## Key Features

1. **Typed DOM Wrappers**  
   Classes like `Document` and `Element` that let you manipulate attributes, properties, and children in C#.

2. **Event Handling**  
   Register and remove JavaScript event listeners from C# methods. Use delegates to handle browser events.

3. **“Component” Model**  
   A base class (`JSVComponent`) and derived examples (`JSVButton`, `JSVSlider`, etc.) that render HTML and maintain internal state via C# DTOs.

4. **Transformer System**  
   `JSVTransformer` classes can convert JSON payloads to strongly typed C# objects, avoiding repeated code in JS.

5. **Source-Generated JSON**  
   Minimizes reflection overhead with `[JsonSerializable]` contexts.

6. **Incremental & Non-Invasive**  
   Compatible with your existing Razor Pages — no full re-architecture required.

---

## How It Works

### 1. C# Interop Classes (JSVaporizer)

Within the **`JSVaporizer`** namespace, you’ll find:

- **`WasmElement`, `WasmDocument`, `WasmWindow`** partial classes:  
  - Each class has `[JSImport(...)]` attributes pointing to JavaScript functions (e.g. `element.js`, `document.js`, `window.js`).  
  - This is your typed interface to perform actions like `SetAttribute`, `AppendChild`, `AddEventListener`, or `alert(...)`.

- **`JSVEventHandlerPool`** and **`JSVGenericFuncPool`**:  
  - Store C# delegates for event callbacks or generic function calls.  
  - When a JS event occurs, it calls into these pools to find the matching delegate and executes it in C#.

- **`Document`** and **`Element`** classes:  
  - Provide a friendlier abstraction on top of the lower-level `[JSImport]` calls.  
  - For example, `element.SetAttribute("class", "btn")` or `element.AppendChild(childElem)`.

These classes **mirror** the DOM but **in C#**. You can think of them as a “bridging layer” between .NET code and JavaScript.

### 2. UI Components (JSVNuFlexiArch)

Under the **`JSVNuFlexiArch`** namespace, you’ll find:

- **`JSVComponent`**:  
  - An abstract class with lifecycle methods (`Initialize`, `UpdateState`, `GetState`, `RenderBody`).  
  - Each subclass typically has an associated DTO (`CompDataDto`) to represent its state (e.g. label, isChecked, etc.).

- **Derived Components** (e.g., `JSVButton`, `JSVCheckbox`, `JSVSlider`, `JSVTextInput`, etc.):  
  - Each has typed properties for user interaction, sets up event listeners, and renders HTML from C#.  
  - They rely on the DOM interop in `JSVaporizer` behind the scenes.

- **`JSVComponentMaterializer`**:  
  - Dynamically **instantiates** components from assembly-qualified names, sets their state (DTO), and **injects** them into the DOM.  
  - This allows you to pass JSON describing the component and its state, and let the library handle the entire process.

### 3. JavaScript Modules

In your **`wwwroot`** (or similar) folder, you’ll have:

- **`jsv_init.js`**:  
  - Bootstraps the .NET WASM runtime (`dotnet.js`), loads the `"JSVaporizer.NET.8"` assembly (adjust if needed), and then calls `setModuleImports(...)` to bind your partial classes (`WasmElement`, etc.) to actual JS implementations.

- **`for_dotnet/`** folder containing files like:  
  - **`element.js`, `document.js`, `window.js`, `js_function_pool.js`**  
  - Each exports specific functions (e.g. `createJSVaporizerElement`, `getPropertyNamesArray`, `alert(...)`) that your `[JSImport("...", "element")]` or `[JSImport("...", "window")]` calls in C# need.

These JS files **do not** contain business logic — they simply provide a minimal layer for DOM manipulation and event handling, deferring actual logic to your C# code.

---

## Usage

### 1. Project Setup

1. **Add the C# Projects**  
   Include references to the `JSVaporizer` and `JSVNuFlexiArch` projects in your .NET solution.  
   Make sure you’re targeting .NET 7 or higher (with WebAssembly support).

2. **Include the JavaScript Files**  
   - Place `jsv_init.js`, plus the `for_dotnet/*.js` files in a location served by your web app (e.g., `wwwroot/js/`).  
   - Adjust relative imports in `jsv_init.js` to match your directory structure.

3. **Configure .NET WASM Boot**  
   - Typically, you’ll have a `<script>` tag referencing `dotnet.js` from the official WASM runtime, then a `<script>` for your `jsv_init.js`. This ensures the runtime is loaded before your initialization code.

### 2. Initialization

In a Razor file or `_Layout.cshtml`:

```html
<script src="_framework/dotnet.js" defer></script>
<script src="js/jsv_init.js" defer></script>
```

Make sure the script references are in the correct order so the `.NET` runtime is available to `jsv_init.js`.

### 3. Creating and Using Components

#### Manual Instantiation

```csharp
// Create a new JSVTextInput with a unique prefix
var textInput = new JSVTextInput("myTextInput");

// Update its state (label, initial text)
textInput.UpdateState(new TextInputDataDto {
    Label = "Enter your name:",
    InputValue = ""
});

// Render it into an existing DOM element "placeholder"
JSVComponentMaterializer.Render(
    "myTextInput",
    textInput,
    "placeholder",
    append: false
);
```

#### JSON + Materializer

```csharp
// Suppose you have a JSON describing both metadata and state
string sliderInstanceJson = "{ \"MetadataJson\": \"...\", \"StateJson\": \"...\" }";

// Dynamically create & render the component described by JSON
bool success = JSVComponentMaterializer.InstantiateAndRenderFromJson(
    sliderInstanceJson,
    referenceElementId: "sliderContainer",
    append: true
);
```

### 4. Event Handling

Inside a component, you can register events like so:

```csharp
// C# event handler
EventHandlerCalledFromJS clickHandler = (elem, eventType, evnt) =>
{
    Console.Log("Button clicked in C#");
    // Return behavior: e.g., prevent default & stop propagation
    return (int)JSVEventHandlerBehavior.NoDefault_NoPropagate;
};

// Attach it to your element
Element buttonElem = Document.AssertGetElementById("myButtonId");
buttonElem.AddEventListener("click", "buttonClickKey", clickHandler);
```

When the user clicks, the JavaScript code in `element.js` calls back into C# via `CallJSVEventHandler(funcKey, elem, eventType, event)`.

---

## Advantages

1. **Incremental, Non-Invasive**  
   Continue using Razor Pages. Introduce client-side C# logic only where needed.

2. **Reduced JavaScript Duplication**  
   Keep DTOs and validation in strongly typed C#, rather than rewriting them in JS.

3. **Fine-Grained DOM Control**  
   Directly manipulate attributes, events, etc. No large abstraction layers.

4. **No Full Blazor Requirement**  
   If Blazor is impractical or you only want certain .NET features, this toolkit gives a simpler approach.

5. **Source-Generated Serialization**  
   Efficient `[JsonSerializable]` contexts to avoid reflection overhead.

---

## Limitations and Why Not to Use

1. **Not a Full Framework**  
   No built-in routing, no large ecosystem or official docs beyond this. Large-scale apps might prefer a bigger solution.

2. **Limited Community**  
   This is a custom toolkit. If you need a robust community or official support, consider Blazor or another mainstream framework.

3. **Potential Key Collisions**  
   Reusing the same `funcKey` for multiple listeners is disallowed unless you remove the previous one. By design, but could be restrictive.

4. **Modern Browser Dependence**  
   Uses ES modules and dynamic imports; older browsers need polyfills or won’t work smoothly.

5. **Performance / Startup**  
   All .NET WASM solutions have an initial load cost. If minimal footprint or maximum speed is critical, pure JS might be better.

6. **Requires Setup**  
   Must load `.NET WASM` and your JS modules in the correct order. This is more manual than a typical Blazor or React app.

---

## Examples

### Simple JSVButton

```csharp
public void BuildButton()
{
    var button = new JSVButton("myButton");
    button.UpdateState(new ButtonDataDto {
        Text = "Click Me!",
        Label = "Optional Label"
    });

    // Insert into the DOM
    JSVComponentMaterializer.Render(
        "myButton",
        button,
        "buttonContainer",
        append: false
    );
}
```

### Slider with Event Sync

```csharp
public void BuildSlider()
{
    var slider = new JSVSlider("volumeSlider");
    slider.UpdateState(new SliderDataDto {
        Label = "Volume",
        Value = 50,
        MinValue = 0,
        MaxValue = 100,
        Step = 1
    });

    // Renders into an existing div with ID="sliderPlaceholder"
    JSVComponentMaterializer.Render(
        "volumeSlider",
        slider,
        "sliderPlaceholder",
        append: true
    );
}

// The 'Initialize()' method in JSVSlider automatically
// wires up "change" and "Reset" button events internally.
```

---

## Contributing

1. **Issues**  
   - Feel free to open an issue if you find a bug or have a feature request. Provide as much detail as possible.

2. **Pull Requests**  
   - Fork, add or improve components, then open a pull request. We welcome any additions or corrections.

3. **Style & Conventions**  
   - Follow .NET naming standards, keep the JavaScript minimal, and maintain clarity throughout the codebase.

4. **Roadmap**  
   - Potential expansions: advanced event systems, more components, or a more sophisticated approach to disposing/unmounting components.

**Thank you for considering JSVaporizer & JSVNuFlexiArch!**  
If you need an incremental way to unify your client-side and server-side C# logic without fully embracing a heavier framework like Blazor, this toolkit might be the right fit. Otherwise, if you need official support or advanced features (like SSR, routing, large ecosystems), you might be better served by Blazor or a mainstream JS framework.
