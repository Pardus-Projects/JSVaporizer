# **JSVaporizer** 
A Lightweight .NET WASM Framework for Handcrafted UI Components

## Overview

**JSVaporizer** is a minimalistic, highly flexible framework for building interactive web UIs in .NET using WebAssembly, without the overhead of a more opinionated system like Blazor. It provides:

1. **Simple, Component-Based Architecture**:  
   - Define your own UI components by subclassing `JSVComponent`.  
   - Use Handlebars templates to generate markup on either the server or client.

2. **Direct DOM Interaction**:  
   - Bypass the typical virtual DOM or heavy data-binding layers.  
   - Manipulate DOM elements using typed C# classes (`Document`, `Element`, etc.) and low-level `[JSImport]`/`[JSExport]` interop.

3. **Event-Handler Pooling**:  
   - Attach .NET delegates to DOM events (e.g., “click,” “change”) through a function pool keyed by a unique string, eliminating extensive JavaScript boilerplate.

4. **Server + Client Rendering**:  
   - Render partial HTML on the server (Razor Pages or MVC) and then enhance or replace portions of the DOM at runtime on the client, purely in C#.

By focusing on direct control and a straightforward lifecycle (“Build → Insert → Wire up events”), **JSVaporizer** offers a “do-it-yourself” style for developers who prefer minimal abstraction while still enjoying strongly-typed .NET code and tooling.

---

## Table of Contents

- [Key Features](#key-features)  
- [Getting Started](#getting-started)  
  - [Installation](#installation)  
  - [Basic Usage](#basic-usage)  
- [Core Concepts](#core-concepts)  
  - [JSVComponent](#jsvcomponent)  
  - [Templates with Handlebars](#templates-with-handlebars)  
  - [JSVCompBuilder and Post-Attach Setup](#jsvcompbuilder-and-post-attach-setup)  
  - [DOM Wrappers (Document & Element)](#dom-wrappers-document--element)  
  - [Event Handling](#event-handling)  
- [Creating a New Component](#creating-a-new-component)  
- [Examples](#examples)  
- [FAQ](#faq)  
- [Contributing](#contributing)  
- [License](#license)
- [TODO](#todo)

---

## Key Features

- **Lightweight**: Only the minimal set of classes you need to interface with JS from .NET, manipulate DOM elements, and render HTML via templates.  
- **Flexible Rendering**: Use the same component model for server-side generation (Razor) or client-side dynamic insertion.  
- **Typed DOM Access**: `Element` and `Document` classes mimic common web APIs but in a strictly typed C# manner, letting you do things like `element.SetAttribute("class", "my-class")` or `document.AssertGetElementById("someId")`.  
- **No Heavy Dependencies**: Relies on the new .NET 7/8 `[JSExport]` / `[JSImport]` attributes for WASM interop—no bulky frameworks.  
- **Event Delegation in C#**: Add or remove event listeners in the browser with a one-liner in .NET code, thanks to the event-handler pool.

---

## Getting Started

### Installation

1. **Clone or Add Reference**  
   - Copy the source files into your project, or reference the compiled library (if you’ve built a NuGet package, install it via `dotnet add package JSVaporizer`).
   
2. **Ensure .NET 7+**  
   - JSVaporizer heavily relies on the .NET WASM interop features introduced in .NET 7. Make sure your project targets `.NET 7` or `.NET 8`.

3. **Use a Supported Browser**  
   - WebAssembly with JavaScript interop is widely supported in modern browsers. No special polyfills should be required beyond typical .NET WASM support.

### Basic Usage

1. **Define a Component**  
   ```csharp
   public class MyLabel : JSVComponent
   {
       public string LabelId { get; }

       public MyLabel(string uniqueName) : base(uniqueName)
       {
           LabelId = UniqueWithSuffix("LabelId");
       }

       protected override string GetTemplate()
       {
           return $@"
               <span id=""{{{UniqueName}}}"">
                   <label id=""{{{LabelId}}}"">Hello from JSVaporizer</label>
               </span>
           ";
       }
   }
   ```

2. **Render on the Server**  
   In a Razor Page:
   ```csharp
   @page
   @model IndexModel

   @{
       var labelComp = new MyLabel("myUniqueLabel");
   }

   <h1>Hello World with JSVaporizer</h1>
   @labelComp.RenderBuilder()
   ```

3. **Replace or Insert on the Client**  
   If you have a placeholder `<div id="myPlaceholder"></div>` in the HTML, you can call:
   ```csharp
   JSVCompBuilder.Invoke(
       "myUniqueLabel",
       typeof(MyLabelBuilder).AssemblyQualifiedName,
       "myPlaceholder"
   );
   ```
   and the placeholder’s content becomes the component’s rendered HTML.

---

## Core Concepts

### JSVComponent

- **Base class** for all components in JSVaporizer.  
- Stores a `uniqueName` and provides the `Render()` method that calls a Handlebars template (`GetTemplate()`).
- Example Subclasses: `CheckBox`, `TextInput`, `Button`, `DropDownList`.

### Templates with Handlebars

- Components override `GetTemplate()` to return a Handlebars string:
  ```csharp
  protected override string GetTemplate()
  {
      return @"
          <span id=""{{{UniqueName}}}"">
              <input type=""checkbox"" id=""{{{CheckBoxId}}}"" />
          </span>
      ";
  }
  ```
- **Why Handlebars?**  
  - Simplifies inline expressions like `{{MyProperty}}` or loops with `{{#each}}`.
  - You still get full C# control over property data.

### JSVCompBuilder and Post-Attach Setup

- **`JSVCompBuilder`**: A pattern for creating a component, setting default properties, and then inserting it into the DOM.  
- **`PostAttachToDOMSetup`**: An optional delegate that runs after the component’s HTML is in the page, ideal for binding event handlers.  
  ```csharp
  public PostAttachToDOMSetup? PostAttachToDOMSetup;
  ```

### DOM Wrappers (Document & Element)

- `Document` provides methods like `CreateElement(...)`, `GetElementById(...)`, or `GetElementsByTagName(...)`, each returning an `Element`.
- `Element` wraps a DOM element and offers:
  - `.SetProperty(...)`, `.GetProperty(...)`  
  - `.SetAttribute(...)`, `.GetAttribute(...)`  
  - `.AddEventListener(...)`, `.RemoveEventListener(...)`  
- Under the hood, it uses `[JSImport]` calls to interact with the browser’s DOM API.

### Event Handling

- **AddEventListener**: 
  ```csharp
  myButton.AddEventListener("click", "myClickKey", (elem, eventType, evnt) =>
  {
      // C# logic for click
      Console.Log("Button was clicked!");
      return (int)JSVEventHandlerBehavior.NoDefault_NoPropagate;
  });
  ```
- This automatically registers an event handler in the `WasmJSVEventHandlerPool`, letting the browser route the “click” to your C# method.

---

## Creating a New Component

1. **Subclass `JSVComponent`**  
   ```csharp
   public class FancyParagraph : JSVComponent
   {
       public FancyParagraph(string uniqueName) : base(uniqueName) {}

       protected override string GetTemplate()
       {
           return @"
               <p id=""{{{UniqueName}}}"">A fancy paragraph!</p>
           ";
       }
   }
   ```
2. **Add Interactive Methods** (optional)  
   If you need to manipulate the DOM post-render:
   ```csharp
   [SupportedOSPlatform("browser")]
   public void SetText(string newText)
   {
       Document.AssertGetElementById(UniqueName).SetProperty("textContent", newText);
   }
   ```

3. **Use the New Component**  
   - Server side with `@myParagraph.RenderBuilder()`.  
   - Client side with a `JSVCompBuilder` subclass or direct creation, then inserting via `outerHTML`.

---

## Examples

### A Composite Component

See `MyTestComp` for a larger example that contains:
- A `DropDownList`  
- A `Button`  
- A `List<string>` displayed in a `<ul>`  
- A click event handler that removes itself after firing once.

### A “One-and-Done” Button

- `Button` class lets you show a button with an event handler. In `OnClick`, you can remove the event after triggering once, effectively disabling further clicks.

---

## FAQ

1. **How does this differ from Blazor?**  
   - JSVaporizer is far more manual and lightweight. Blazor does diff-based rendering, has signal-driven re-renders, and includes many abstractions. JSVaporizer basically gives you direct DOM control via .NET. No large runtime overhead or specialized lifecycle.

2. **Can I still use JavaScript libraries alongside it?**  
   - Absolutely. JSVaporizer’s approach is minimal enough to coexist with other JS frameworks or libraries.

3. **Is there any virtual DOM or diffing?**  
   - No. If you need to update, you can either set `.SetProperty("outerHTML", ...)` to fully replace elements or manipulate them directly.

4. **What about server-side only usage (no WASM)?**  
   - You can still use `JSVComponent` to render HTML strings on the server (like Razor partials). The client-side features (events, `Element` wrappers) won’t apply without WASM, though.

---

## Contributing

- **Issues & Features**: Please open issues on GitHub if you find a bug or have a request.  
- **Pull Requests**: We welcome improvements, additional components, or better tooling.  
- **Testing**: Automated tests using [Playwright](https://playwright.dev/dotnet/docs/intro) or similar are recommended to verify DOM interactions in a headless browser.

---

## License

This project is provided under the [MIT License](./LICENSE), meaning it’s free and open-source. Feel free to fork, modify, and adapt it for your own needs.

---

**Enjoy building .NET-based UIs** with direct DOM control using **JSVaporizer**! If you have questions or want to showcase a project that uses it, we’d love to hear from you.

---

## TODO

### Weaknesses & Possible Mitigations

| # | Severity | Weakness (recap) | Mitigation strategy |
|---|:---:|---|---|
| **W‑4** | 7 | **Stringly‑typed Handlebars has no compile‑time checks.** | *Source‑generate templates.* <br>• Use a Roslyn generator that parses the Handlebars file, creates a partial class with strongly‑typed properties, and raises compile errors on missing members. <br>• Or switch to Razor‑class‑library `.razor` templates (they compile to C#). |
| **W‑5** | 7 | **`SetProperty` relies on runtime type‑switch.** | *Introduce generics & converters.* <br>```SetProperty<T>(string name, T value) where T: unmanaged | string | JSObject\n```<br>• Use a `switch` on `typeof(T)` in *one* place; other overloads fan‑in to the generic method. <br>• For uncommon types, allow users to register a custom `IJSVTypeConverter<T>`. |
| **W‑6** | 6 | **Caller must lowercase attribute names.** | *Normalize internally.* <br>• In `SetAttribute` / `HasAttribute` / `GetAttribute`, convert `attrName = attrName.ToLowerInvariant()` before use. <br>• Keep the runtime exception only in DEBUG builds as a developer hint. |
| **W‑8** | 5 | **Global dictionaries grow indefinitely.** | *Weak refs + periodic sweep.* <br>• Store `WeakReference<Element>` / `WeakReference<Delegate>` in the pools. <br>• Run a lightweight sweep every N seconds (or on pool size > X) to drop dead entries. |
| **W‑9** | 5 | **Blocking `alert()` freezes the WASM thread.** | *Non‑blocking UI helpers.* <br>• Ship a tiny JS toast/snackbar helper exposed as `Window.Notify(string, int ms=3000)`. <br>• Mark the old `Alert` API as `[Obsolete]` with a doc comment explaining the perf issue. |
| **W‑10** | 4 | **Hidden‑input plumbing couples views to internals.** | *Decouple host markup.* <br>• Replace hidden `<input>` tags with a single `<script type=\"text/jsv-metadata\">{ JSON }</script>` block that lists builder name + placeholder mapping; parse it at bootstrap. <br>• Or use `data-jsv-*` attributes directly on the placeholder span. |
| ~~**W‑1**~~ FIXED | 9 | ~~**String‑key pools are brittle.**~~ | *Eliminate manual keys.* <br>• Generate a GUID (or incrementing int) on `AddEventListener`/`RegisterFunction`; return that handle to the caller. <br>• Store a `WeakReference` to the delegate so GC can reclaim it. <br>• Wrap the handle in a tiny `struct ListenerId` to regain type‑safety. |
| ~~**W‑2**~~ | 8 | ~~**`JSObject` lifetime is fragile.**~~ | *Centralise proxy handling.* <br>• Introduce an **object cache** that maps element‑ID → live `JSObject`. <br>• Use `using`/`await using` so disposal is deterministic. <br>• Provide an `Element.Dispose()` that nulls the cache entry and detaches listeners. |
| ~~**W‑3**~~ FIXED from W-1| 8 | ~~**Manual listener clean‑up causes leaks.**~~ | *Auto‑detach.* <br>• Make `Element` implement `IDisposable`; in its `Dispose` iterate `_eventListenersByType` and call `RemoveEventListener`. <br>• In browser JS shim, register a `MutationObserver` that detects node removal and notifies WASM to dispose the matching element. |
| ~~**W‑7**~~ | 6 | ~~**`WasmJSVGenericFuncPool.Remove` signature mismatch.**~~ | *API tidy‑up.* <br>• Change signature to `bool Remove(string funcKey)` to mirror `Add`. <br>• Provide an `[Obsolete]` overload for one release cycle to avoid breaking existing callers. |
