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

The `JSVaporizer.NET.8.dll` embeds the necessary WASM JavaScript files.  
See below.

---

## Usage

### 1. Project Setup

1. **Add the C# Projects**  
   Include references to the `JSVaporizer` and `JSVNuFlexiArch` projects in your .NET solution.  
   Make sure you’re targeting .NET 7 or higher (with WebAssembly support).

2. **Include the JavaScript Files**  
   - The `JSVaporizer.NET.8.dll` embeds the necessary WASM JavaScript files:
   - Allow `ASP.NET Core` to serve these files by adding the following to `Program.cs`
      ```
      // Required to serve WASM assembly to client.
      app.UseStaticFiles(new StaticFileOptions
      {
         ServeUnknownFileTypes = true
      });

      // Extract embedded jsvwasm JavaScript files from JSVaporizer.NET.8 assembly.
      Assembly ass = typeof(JSVapor).GetTypeInfo().Assembly;
      EmbeddedFileProvider embProv = new EmbeddedFileProvider(ass, "JSVaporizer.NET.8.jsvwasm");
      //var verifyFiles = embProv.GetDirectoryContents(""); // For debugging breakpoint
      app.UseStaticFiles(new StaticFileOptions()
      {
         FileProvider = embProv
      });
      ```

3. **Expose the WASM to your front end**  
   - Add the following to `site.js` or similar:
      ```
      let JsvWasm = null;

      export async function GetJsvWasm() {
         if (JsvWasm == null) {
            await import("../jsvwasm.js").then((jsvWasm) => {
                  JsvWasm = {
                     ExportConfig: jsvWasm.jsvExportConfig,
                     RegisterCustomImports: jsvWasm.jsvRegisterCustomImports,
                     RegisterJSFunction: jsvWasm.jsvRegisterJSFunction,
                     CallJSVGenericFunction: jsvWasm.callJSVGenericFunction,
                     GetExportedAssembly: jsvWasm.GetExportedAssembly
                  };
            });
         }

         return JsvWasm;
      }
      ```

Make sure the script references are in the correct order so the `.NET` runtime is available to `jsv_init.js`.

### 2. Creating and Using Components

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

#### JSON + JSVComponentMaterializer

1. In a `Sdk="Microsoft.NET.Sdk.WebAssembly"` project (named `MyViewLib` in this example):
   ```
   public partial class JSVComponentInitializer
   {
      [JSExport]
      [SupportedOSPlatform("browser")]
      public static bool InstantiateAndRenderFromJson(string instanceDtoJson, string referenceElementId)
      {
         return JSVComponentMaterializer.InstantiateAndRenderFromJson(instanceDtoJson, referenceElementId);
      }
   }
   ```

2. In **`Foo.cshtml`** 
   ```razor
   <h2>JSVSlider</h2>
   <input id="hf_JSVSlider_InstanceJson" type="hidden" value="@Model.JSVSlider_InstanceJson" />
   <div id="JSVSlider_Placeholder"></div>
   ```

3. In **`foo.js`** 
   ```
   let site;

   import("./site.js").then((module) => {
      site = module;

      // Launch your front end here
      LaunchApp();
   });

   async function LaunchApp() {
         let JSVSlider_InstanceJson = $("#hf_JSVSlider_InstanceJson").val();
      resStr = jsvExports.MyViewLib.JSVComponentInitializer.InstantiateAndRenderFromJson(JSVSlider_InstanceJson, "JSVSlider_Placeholder");
      alert("It worked.");
   }
   ```


### 3. Event Handling

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
