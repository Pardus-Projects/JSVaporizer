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
   - Attach .NET delegates to DOM events (e.g., “click,” “change”) through a function pool, eliminating extensive JavaScript boilerplate.

4. **Server + Client Rendering**:  
   - Render partial HTML on the server (Razor Pages or MVC) and then enhance or replace portions of the DOM at runtime on the client, purely in C#.

By focusing on direct control and a straightforward lifecycle (“Build → Insert → Wire up events”), **JSVaporizer** offers a do-it-yourself style for developers who prefer minimal abstraction while still enjoying strongly-typed .NET code and tooling.

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
- **No Heavy Dependencies**: Relies on the new .NET 8 `[JSExport]` / `[JSImport]` attributes for WASM interop—no bulky frameworks.  
- **Event Delegation in C#**: Add or remove event listeners in the browser in .NET code.

---

## Getting Started

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
               <span id=""{{UniqueName}}"">
                   <label id=""{{LabelId}}"">Hello from JSVaporizer</label>
               </span>
           ";
       }
   }
   ```

2. **Replace or Insert on the Client**  
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
          <span id=""{{UniqueName}}"">
              <input type=""checkbox"" id=""{{CheckBoxId}}"" />
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
  myButton.AddEventListener("click", (elem, eventType, evnt) =>
  {
      // C# logic for click
      Console.Log("Button was clicked!");
      return (int)JSVEventHandlerBehavior.NoDefault_NoPropagate;
  });
  ```
- This automatically registers an event handler in the `WasmJSVEventHandlerPool`, letting the browser route the click to your C# method.

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
               <p id=""{{UniqueName}}"">A fancy paragraph!</p>
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

## TODO

## DOM‑Component Guardrails

### Purpose
Provide a minimal policy that protects JSVaporizer from destructive DOM mutations **without** introducing a heavyweight virtual DOM.

* Keep the framework lightweight (no diff engine or per‑element `MutationObserver`s by default).
* Allow benign DOM edits (browser autofill, analytics attributes, A11y extensions).
* Fail‑fast only on mutations that break JSVaporizer’s bookkeeping (root removal, `id` corruption).
* Offer opt‑in strictness & extensibility for power users.

---

### Guardrail Principles
| ID | Principle | Default Action | Opt‑In Strict Action |
|----|-----------|----------------|---------------------|
| **P‑1** | Component **root element must remain connected** to the document. | **Log & dispose** wrapper if root permanently detached. | **Throw `DomRootRemovedException`** immediately. |
| **P‑2** | Each managed element must have a **stable, unique `id`**. | **Update** wrapper on `id` change; log on duplicate. | **Throw `DuplicateIdException`** or `IdClearedException`. |
| **P‑3** | All other mutations (class, style, children, text) are **allowed**. | No action. | Throw if any external mutation touches the subtree. |
| **P‑4** | Components may override and reconcile via **`OnDomMutated` hook**. | No‑op by default. | Same. |

---

### Feature Matrix
| Feature | Lightweight Mode *(default)* | Strict Mode | Notes |
|---------|------------------------------|-------------|-------|
| Global `MutationObserver` | ✔ | ✔ | Single observer on `document.documentElement`. |
| Per‑element observers | ✖ | ✖ | Kept internal for debugging. |
| `OnDomMutated` hook | Available | Available | Fires after global observer batching. |
| Dispose on root removal | Automatic | Automatic | Plus exception in strict mode. |
| `id` duplicate detection | Debug assert | Exception | Uses `_jsvElements` dictionary. |
| Config toggle | `JSVapor.Config.StrictDom = false` | `true` | Can also be set via environment variable. |

---

### Coding Roadmap (high‑level)
1. **JS – Refactor Observer**
   * *`mutation.js`* exports `raiseDomRemovedById` & `raiseDomIdChanged` with batching and move‑detection.
   * Load once during bootstrap; no per‑component observers.

2. **Imports.cs**
   ```csharp
   [JSImport("raiseDomRemovedById", "document")]  internal static partial void RaiseDomRemovedById(string[] ids);
   [JSImport("raiseDomIdChanged",   "document")]  internal static partial void RaiseDomIdChanged(string?[] oldIds, string?[] newIds);
   ```

3. **Document.cs** – new helpers
   ```csharp
   internal static bool TryGetWrapper(string id, out Element e) => _jsvElements.TryGetValue(id, out e);

   internal static void RemoveWrapper(string id) => _jsvElements.Remove(id);
   ```

4. **Exports.cs** - new helpers
   ```csharp
   [JSExport("raiseDomRemovedById")]
   public static void RaiseDomRemovedById(string[] ids)
   {
       foreach (var id in ids)
       {
           if (TryGetWrapper(id, out var elem))
               elem.DisposeInternal();                        // detaches listeners etc.
       }
   }

   [JSExport("raiseDomIdChanged")]
   public static void RaiseDomIdChanged(string?[] oldIds, string?[] newIds)
   {
       for (int i = 0; i < oldIds.Length; i++)
       {
           var oldId = oldIds[i];
           var newId = newIds[i];
           if (oldId is null) continue;                      // newly added id
           if (TryGetWrapper(oldId, out var elem))
           {
               elem.UpdateId(oldId, newId);
               elem.OnIdChanged(oldId, newId);
           }
       }
   }
   ```

5. **Element.cs** – new APIs
   ```csharp
   internal void UpdateId(string oldId, string? newId)
   {
       Document.RemoveWrapper(oldId);
       if (!string.IsNullOrEmpty(newId))
           Document.RegisterWrapper(newId!, this);
       Id = newId ?? string.Empty;
   }

   protected virtual void OnDomElementRemoved() { }
   protected virtual void OnIdChanged(string oldId, string? newId) { }
   ```

6. **Strict Mode Toggle**
   ```csharp
   public static class Config
   {
       public static bool StrictDom { get; set; } =
           bool.TryParse(Environment.GetEnvironmentVariable("JSVAPOR_STRICT_DOM"), out var b) && b;
   }
   ```
   *Throw exceptions instead of logging based on `Config.StrictDom`.*

7. **Debug Assertions (DEBUG builds only)**
   ```csharp
   Debug.Assert(!Document._jsvElements.ContainsKey(newId!), "Duplicate id detected: " + newId);
   ```

8. **JSVComponent hook** (optional for devs)
   ```csharp
   protected virtual void OnDomMutated(DomMutationRecord rec) { }
   ```

---

### Next Steps
* **Implement code steps 1‑4**
* Validate with test matrix:
  1. Remove component → wrapper disposed.
  2. Move component → no disposal/log.
  3. Change id → wrapper map updates.
  4. Duplicate id in strict mode → exception.
* Document `StrictDom` flag and `OnDomMutated` hook in README.


