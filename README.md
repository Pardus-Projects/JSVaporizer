# JSVaporizer & JSVNuFlexiArch

This repository provides a custom .NET WebAssembly interop layer (in the `JSVaporizer` namespace) and a small component framework (in the `JSVNuFlexiArch` namespace) for building interactive web UI entirely in C#. The code demonstrates how to directly call and expose JavaScript functions using `[JSImport]` and `[JSExport]`, while managing DOM elements, events, and state in C#.

## Table of Contents

- [Overview](#overview)
- [Architecture](#architecture)
  - [JSVaporizer (Interop Layer)](#jsvaporizer-interop-layer)
  - [JSVNuFlexiArch (Components)](#jsvnuflexiarch-components)
  - [Transformer Architecture](#transformer-architecture)
- [Key Features](#key-features)
- [Usage](#usage)
  - [Prerequisites](#prerequisites)
  - [Building and Running](#building-and-running)
  - [Basic Example](#basic-example)
- [Creating Custom Components](#creating-custom-components)
- [Known Limitations / Caveats](#known-limitations--caveats)
- [Contributing](#contributing)
- [License](#license)

---

## Overview

This project shows how to integrate .NET code with browser APIs via `[JSExport]` and `[JSImport]`, as well as how to implement a basic “component” system. **JSVaporizer** is the low-level library that calls or is called by JavaScript, while **JSVNuFlexiArch** provides:
1. A small model for defining and rendering UI components (buttons, checkboxes, sliders, etc.).
2. A “transformer” mechanism for converting JSON data into a component view and vice versa.

The code is particularly useful if you want to avoid a JavaScript-based front-end framework or the full Blazor stack, and instead manually manage DOM elements from C#.

---

## Architecture

### JSVaporizer (Interop Layer)

- **Interop Classes**  
  - `WasmElement`, `WasmDocument`, `WasmWindow`, etc. wrap DOM APIs using `[JSImport("functionName", "moduleName")]`.
  - `WasmJSVEventHandlerPool`, `WasmJSVGenericFuncPool` maintain dictionaries of C# delegates for event handling or generic function callbacks keyed by a string.  
- **`Element` Class**  
  - Manages references to browser DOM objects. Uses ephemeral `JSObject` references that are disposed once the element is attached to the DOM. Further calls use the element’s `Id` to look up a fresh `JSObject`.  
- **Event Handling**  
  - Events are registered via `AddEventListener(string eventType, string funcKey, EventHandlerCalledFromJS handler)`.  
  - The event callback is passed from JS back to C# using `[JSExport]`, then triggers the appropriate function from `WasmJSVEventHandlerPool`.

### JSVNuFlexiArch (Components)

- **`JSVComponent`**: An abstract base class representing a “component” with:
  - A `Metadata` object that stores key-value pairs (such as a unique prefix or the assembly-qualified type name).
  - Methods to render HTML (`Renderer.Render()`), update state (`UpdateState()`), and retrieve state (`GetState()`).
  - JSON serialization hooks to store/restore component data (`SerializeState()` / `DeserializeState()`).
- **Renderers**  
  - Components define how they generate DOM elements (labels, inputs, etc.) via classes like `JSVButtonRenderer`, `JSVCheckboxRenderer`, etc.
  - `JSVComponentRenderer` handles the basic pattern of writing opening and closing `<div>` tags, while a subclass’s `RenderBody(...)` supplies the inner HTML.
- **Concrete Components**  
  - **`JSVButton`**: Renders a `<button>` with an optional label.  
  - **`JSVCheckbox`**: Renders an `<input type="checkbox">` plus label.  
  - **`JSVSlider`**: Renders an `<input type="range">` plus label and displays the current numeric value.  
  - **`JSVTextDisplay`**: Renders text inside a `<span>`.  
  - **`JSVTextInput`**: Renders an `<input type="text">` plus an optional label.

### Transformer Architecture

- **`JSVTransformer`**  
  - An abstract class for taking a JSON string (a DTO) and turning it into a view representation, or converting the current view back into a DTO.  
  - Each transformer implements methods such as `JsonToDto(string dtoJson)`, `DtoToView(string dtoJson, string? userInfoJson = null)`, and `ViewToDto()`.  
  - A typical flow is:  
    1. **Load** a JSON string.  
    2. **Convert** it to a data object (`TransformerDto`).  
    3. **Render** or **update** a UI component.  
    4. **Capture** changes from the UI back into a DTO.  
- **`TransformerDto`**  
  - A base or marker class for data objects used in transformation. Your custom transformers may define subclasses representing specific data structures.  
- **`TransformerRegistry`**  
  - Maintains a dictionary of transformers (`JSVTransformer` instances) keyed by a string (e.g., a name or identifier).  
  - Allows you to look up the correct transformer at runtime (`transformerRegistry.Get(xFormerName)`) and invoke it.  
  - Provides a static `Invoke(...)` method to load a transformer by name, run `DtoToView(...)`, and return the resulting HTML or other data.

---

## Key Features

1. **Low-Level DOM Control**  
   Read/write DOM properties (e.g., `.innerHTML`, `.disabled`, `.value`), manage attributes, and register/unregister event listeners entirely in C#.

2. **Events and Callbacks**  
   The dictionary-based “pool” mechanism lets you register event listeners with unique “function keys,” bridging from JavaScript events back to C#.

3. **Ephemeral `JSObject` Usage**  
   Minimizes memory usage by disposing of expensive `JSObject` references after each operation and re-fetching them by ID when needed.

4. **Customizable Components**  
   Each component encapsulates its state in a data-transfer object (DTO) and can be easily extended or combined. State changes are reflected in the DOM and vice versa.

5. **Source-Generated JSON**  
   Uses `[JsonSerializable]` attributes for efficient serialization without reflection. Each component’s data DTO has its own generated context.

6. **Flexible Transformers**  
   Implement your own `JSVTransformer` to handle custom data flows from JSON → component → JSON, using the `TransformerRegistry` to retrieve and apply them on demand.

---

## Usage

### Prerequisites

- .NET 7 or higher SDK (for WebAssembly support and `[JSImport]` / `[JSExport]` usage).
- A compatible toolchain for .NET WebAssembly projects, such as a Blazor WebAssembly app or a console-like WASM app with the right build targets.
- A modern browser that supports WebAssembly.

### Building and Running

1. **Clone or copy** this repository into your local environment.  
2. **Open** the `.csproj` in Visual Studio, VS Code, or your preferred editor.  
3. **Build** the project:
   ```bash
   dotnet build
   ```
4. If this is part of a Blazor WebAssembly app, **run** it:
   ```bash
   dotnet run
   ```
5. **Open** the site in your browser. The .NET → JavaScript bridging code should load automatically.

### Basic Example

Suppose you want to create and render a `JSVButton`:

```csharp
// Example usage in a Blazor page or other .NET WASM entry point:

// 1. Create a unique name for the button.
string uniqueName = "MyFirstButton";

// 2. Instantiate the button.
JSVButton button = new JSVButton(uniqueName);

// 3. Render the button into a container element in the DOM.
bool appended = JSVComponentMaterializer.Render(uniqueName, button, "someContainerId", append: true);

// 4. Update the button’s state from a DTO.
var buttonDto = new ButtonDataDto { Label = "Click me!", Text = "Submit", IsDisabled = false };
button.UpdateState(buttonDto);

// 5. Optionally wire up additional events in the Initialize() method or manually.
```

In this snippet, the code uses your existing `JSVComponentMaterializer.Render(...)` method to generate HTML for the button and place it into a DOM element with ID `someContainerId`.

---

## Creating Custom Components

1. **Inherit** from `JSVComponent`.  
2. **Provide** a constructor that sets up a unique prefix and assigns a renderer:
   ```csharp
   public class MyNewComponent : JSVComponent
   {
       public MyNewComponent(string uniqueName)
       {
           Renderer = new MyComponentRenderer(); // custom renderer
           Metadata.Add("UnqPrefix", uniqueName);
           Metadata.Add("CompTypeAQN", GetAssemblyQualifiedName());

           // ... Additional setup ...
       }
   }
   ```
3. **Implement** methods like `UpdateState(...)` and `GetState()` to push or pull data from the DOM.  
4. **Create** a renderer subclass that overrides `RenderBody(...)`:
   ```csharp
   public class MyComponentRenderer : JSVComponentRenderer
   {
       protected override void RenderBody(JSVComponent tmpComp, HtmlContentBuilder htmlCB)
       {
           // Build the HTML for your component, e.g.:
           htmlCB.AppendHtml($"<div id=\"{...}\">Hello World</div>");
       }
   }
   ```
5. **Use** `[JsonSerializable]` attributes if you want a strongly typed DTO for your component’s state.  
6. **Test** the component in your host application (e.g., a Blazor WASM app).

---

## Known Limitations / Caveats

- **Event Key Collisions**: Each event listener is associated with a string `funcKey`. Make sure these keys remain unique or you could overwrite an existing handler.  
- **Case-Sensitive Attributes**: In the `Element` class, attributes must be lowercase, reflecting how browsers typically normalize attribute names.  
- **Renaming IDs**: Changing an element’s `id` at runtime is not supported because the library uses IDs to track DOM references.  
- **`GetText()` Unimplemented**: In some components (e.g., `JSVTextDisplay`), certain getter methods (`GetText()`) are placeholders with `NotImplementedException`. If you need that functionality, you’ll need to implement it.  
- **Performance Overhead**: Because ephemeral `JSObject` references are disposed after each usage, the code often re-fetches DOM elements by ID. This is simpler to manage but can be slower with frequent property reads/writes.  
- **Transformer Keys**: If you use multiple transformers, ensure your registry keys (the `xFormerRegistryKey`) are unique to avoid collisions in `TransformerRegistry`.  

---

## Contributing

Contributions are welcome! If you find a bug or want to request a feature, please open an issue or submit a pull request. To contribute:

1. **Fork** this repository.  
2. **Create** a new branch for your changes.  
3. **Commit** and push to your fork.  
4. **Open** a pull request describing the changes.

---

## License

*(If you have a specific open-source license, place it here. For example, MIT.)*

This project is licensed under the [MIT License](LICENSE). See the `LICENSE` file for details.
