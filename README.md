# JSVaporizer-ZenView

A **mini-framework** for .NET WebAssembly (WASM) that unifies **server-side** Razor Pages logic with **client-side** interactivity – all in **C#**. It consists of two parts:

1. **JSVaporizer**: A _bare-metal DOM and JS interop_ toolkit, providing:
   - Ephemeral `JSObject` usage
   - Custom `[JSExport]` / `[JSImport]` bindings
   - Thread-safe function/event pools (string-keyed)
   - Basic wrappers for DOM (`Document`, `Element`), `Window` APIs, and more

2. **ZenView**: A _higher-level “component” system_ built on JSVaporizer:
   - Reflection-based rendering
   - Handlebars.NET templating for server & client HTML generation
   - Composable UI elements (e.g. `TextInput`, `TwoTextInputs`, etc.)

Together, these allow you to place your **business logic, data transformations, and UI rendering** all in **C#**, while still using Razor for pages and HTML structure.

---

## Table of Contents

1. [Features](#features)
2. [Installing & Setup](#installing--setup)
3. [Key Concepts](#key-concepts)
   - [JSVaporizer Facade](#jsvaporizer-facade)
   - [ZenView Components](#zenview-components)
   - [ZVTransform & TransformerRegistry](#zvtransform--transformerregistry)
4. [Usage Examples](#usage-examples)
   - [Server-Side Rendering](#server-side-rendering)
   - [Client-Side Materialization](#client-side-materialization)
   - [Form Handling via Transformers](#form-handling-via-transformers)
5. [Advanced Scenarios](#advanced-scenarios)
   - [Composition](#composition)
   - [DOM Facade Internals](#dom-facade-internals)
   - [Function & Event Pools](#function--event-pools)
6. [Comparison to Blazor](#comparison-to-blazor)
7. [FAQ](#faq)
8. [License](#license)

---

## Features

- **Shared C# Logic**: Write validation or business rules once; run them _client-side_ in WASM or _server-side_ via Razor Pages.
- **Low-Level DOM Control**: Directly query/manipulate the DOM using ephemeral `JSObject` handles, or wrap it in higher-level ZenView components.
- **Component-Based UI**: Build and nest UI components with reflection-based “auto-rendering” and Handlebars templates.
- **Incremental Migration**: Gradually replace “JavaScript spaghetti” in Razor pages. Keep your existing HTML and only adopt WASM logic where needed.
- **No Full Rewrite**: Works seamlessly with standard ASP.NET Core Razor Pages; you don’t have to jump to a new SPA framework.
- **Extensible**: Add new ZenView components (like `TextInput`, `TextArea`, etc.) or custom Transformers for advanced workflows.

---

## Installing & Setup

1. **Create or Use an ASP.NET Core (Razor Pages, Minimal APIs, MVC) Project**  
   Ensure you have a `.NET 8` environment.

2. **Include the JSVaporizer-ZenView Libraries**  
   - If you have them as **NuGet** packages (hypothetical packages `JSVaporizer` and `ZenView`), reference them in your `.csproj`.
   - Or, if you’re building from source, include the `.csproj` for both libraries in your solution and reference them.

3. **Enable WebAssembly Support**  
   In your `wwwroot` or project structure, ensure you have the **dotnet.wasm** files or `_framework/dotnet.js` that your WASM runtime uses. Typically, you might see them under `_framework/`.

4. **Set Up JavaScript Glue**  
   - Place files like `jsvwasm.js`, `site.js`, etc. in `wwwroot/js/` (or similar).  
   - Load them in your `_Layout.cshtml` or specific pages.
   - Adjust the `import("./site.js")` patterns as desired within those files.

---

## Key Concepts

### JSVaporizer Facade

- **DOM Classes**:  
  - `Document.AssertGetElementById(string id)`  
  - `Element.SetProperty(...)`, `AppendChild(...)`, etc.  
- **Function/Event Pools**:  
  - Store delegates in a dictionary keyed by strings, so JS can call `.NET` or .NET can call `JSFunction` by referencing a key.
- **Window, Console, Alert**:  
  - Minimal wrappers for `window.alert`, `console.log`, etc.

### ZenView Components

- **Subclass `ZenView`** to create a custom component:

   ~~~~csharp
   [SupportedOSPlatform("browser")]
   public class TextInput : ZenView
   {
      public string InputId { get; }

      private string? _inputValue;

      public TextInput(string uniqueName) : base(uniqueName)
      {
         InputId = UniqueWithSuffix("InputId");
      }

      public string GetInputId()
      {
         return InputId;
      }

      public void SetInputVal(string? val)
      {
         _inputValue = val;
         Document.AssertGetElementById(InputId).SetFormElemValue(_inputValue);
      }

      public string? GetInputVal()
      {
         return _inputValue;
      }

      protected override string GetTemplate()
      {
         string hTemplate = Environment.NewLine + @"
               <span id=""{{{UniqueName}}}"">
                  <input id=""{{{InputId}}}"" type=""text""/>
               </span>
         ";

         return hTemplate;
      }
   }
   ~~~~

- **Render**:  
  - **Server**: `myView.RenderBuilder()` in a Razor Page  
  - **Client**: The metadata JSON approach or `[JSExport]` calls that do `MaterializeFromJson(...)`.

### ZVTransform & TransformerRegistry

- **ZVTransform**:  
  - Abstract base for “data in, data out” transformations. For example, a form might define `JsonToDto(...)`, `DtoToView(...)`, `ViewToDto(...)`.
- **TransformerRegistry**:  
  - A dictionary mapping string keys to `ZVTransform` instances.  
  - `Invoke(...)` looks up the transform by name and calls `DtoToView` or similar.

**Use Case**: In the **BarberAppointment** example, we set up a hidden JSON field, then call `.NET` to fill the form fields or validate them, avoiding parallel JavaScript logic.

---

## Usage Examples

### Server-Side Rendering

~~~~csharp
// IndexModel.cs
public class IndexModel : PageModel
{
    public TextInput TextInput_Server = new("TextInput_Server");
    public void OnGet() { /* no special logic here */ }
}

// index.cshtml
@model IndexModel
@{
    // Renders the text input server-side, returning HTML
    var serverSideHtml = Model.TextInput_Server.RenderBuilder();
}
<div>
    @serverSideHtml
</div>
~~~~

### Client-Side Materialization

~~~~csharp
// If you store the metadata JSON from your ZenView:
string metadataJson = Model.TextInput_Client.GetMetadataJson();

// On the Razor page:
<input id="hf_TextInput_Client_MetadataJson" type="hidden" value="@metadataJson" />
<div id="TextInput_Wrapper"></div>
~~~~

Then, in `index.js`:

~~~~js
import("./site.js").then((module) => {
    site = module;
    LaunchApp();
});

async function LaunchApp() {
    let JsvWasm = await site.GetJsvWasm();
    // 'myCompMaterializer' is your .NET method that calls "MaterializeFromJson"
    let metaJson = document.getElementById("hf_TextInput_Client_MetadataJson").value;
    let resStr = myCompMaterializer(metaJson, "TextInput_Wrapper");
    alert("Client-side materialization done! " + resStr);
}
~~~~

### Form Handling via Transformers

~~~~csharp
public class BarberAppointmentTransformer : ZVTransform
{
    public override string DtoToView(string dtoJson, string? userInfoJson = null)
    {
        // 1) Convert JSON -> DTO
        var dto = JsonToDto(dtoJson);
        // 2) Populate form fields
        Element nameElem = Document.AssertGetElementById("txtName");
        nameElem.SetFormElemValue(dto.Name);
        // etc.
        // 3) Attach event handler
        Document.AssertGetElementById("btnBookNow")
                .AddEventListener("click", "btnBookClick", MyClickHandler());
        return "Form is loaded!";
    }

    public override MyCoolTransformerDto ViewToDto() {
       // read from DOM -> build DTO
    }

    private EventHandlerCalledFromJS MyClickHandler() {
        return (elem, eventType, evnt) => {
            var updatedDto = ViewToDto();
            // validation, AjaxPOST, etc.
            return (int)JSVEventHandlerBehavior.NoDefault_NoPropagate;
        };
    }
}
~~~~

---

## Advanced Scenarios

### Composition

You can **nest** multiple ZenView components in a single class:

~~~~csharp
public class TwoTextInputs : ZenView
{
    public TextInput Input1 { get; }
    public TextInput Input2 { get; }

    public TwoTextInputs(string uniqueName) : base(uniqueName)
    {
        Input1 = new(UniqueWithSuffix("Input1"));
        Input2 = new(UniqueWithSuffix("Input2"));
    }

    protected override string GetTemplate() => @"
            <div id=""{{{UniqueName}}}"">
                Input 1
                {{{Input1}}}
                <hr />
                Input 2
                {{{Input2}}}
            </div>
        ";
}
~~~~

When you call `Render()`, the **base** `ZenView` will **detect** that `Input1` and `Input2` are also `ZenView` objects, rendering them in place.

### DOM Facade Internals

- **Ephemeral JSObject**:  
  - Each `Element` fetches a short-lived reference to the underlying DOM node.  
  - Freed (`Dispose()`) after each usage to avoid memory leaks in .NET → JS bridging.

- **Event System**:  
  - `AddEventListener("click", "someKey", handler)` stores your `.NET` delegate in a dictionary.  
  - A JavaScript wrapper calls back into `[JSExport]` with `funcKey = "someKey"`, which looks up the delegate.

### Function & Event Pools

- You register a function:

~~~~csharp
WasmJSVEventHandlerPool.Add("myKey", myDelegate);
~~~~

- Later, JavaScript triggers an event, calls `CallJSVEventHandler("myKey", ...)`, which executes `myDelegate`.

---

## Comparison to Blazor

**Blazor** is a fully integrated component model from Microsoft that includes:

- Automatic diffing & re-rendering
- Razor `.razor` files for components
- Rich ecosystem of libraries

**JSVaporizer-ZenView** is _lighter_ and _more direct_:

- Emphasizes minimal bridging of DOM, ephemeral references
- Leaves you in control of how to structure pages (standard `.cshtml`)
- Good for incremental migration from Razor Pages or partial adoption of WASM logic

---

## FAQ

**1. Do I have to rewrite all Razor pages to use ZenView?**  
No. You can adopt it selectively. Keep the rest of your pages as plain Razor + JS.

**2. Can I do partial page updates?**  
Yes. You can call `Render()` on a subcomponent and inject its HTML into the DOM. Or directly manipulate DOM elements via `JSVaporizer`.


