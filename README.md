# NuFlexiArch + JSVaporizer

Welcome to a minimal-yet-powerful .NET WebAssembly toolkit that unifies **NuFlexiArch** (transformable DTOs, dynamic components) with **JSVaporizer** (structured .NET↔JS interop). This combination aims to give you fine-grained control over DOM manipulation, JSON-based data transformations, and reflection-driven UI components—all while keeping the codebase lean and explicit.

---

## Table of Contents

1. [Overview](#overview)
2. [Distinctive Features](#distinctive-features)
   1. [Ephemeral `JSObject` Management](#ephemeral-jsobject-management)
   2. [Dictionary-Based Callback Pools](#dictionary-based-callback-pools)
   3. [NuFlexiArch Transformer Registry](#nuflexiarch-transformer-registry)
   4. [Reflection-Based Dynamic Component Creation](#reflection-based-dynamic-component-creation)
   5. [Source-Generated JSON + AOT Friendliness](#source-generated-json--aot-friendliness)
   6. [Minimal, Modular DOM Wrapping](#minimal-modular-dom-wrapping)
   7. [Two-Fold Architecture for UI + Data](#two-fold-architecture-for-ui--data)
3. [High-Level Workflow](#high-level-workflow)
4. [Usage Examples](#usage-examples)
5. [Project Structure](#project-structure)
6. [Future Directions](#future-directions)
7. [License](#license)

---

## Overview

- **NuFlexiArch**:
  - Provides an extensible system for converting JSON to DTOs, generating “view” strings from those DTOs, and reflecting them into UI components.  
  - Features a dynamic component model (`AComponent`, `IComponentRenderer`, `CompStateDto`) with custom metadata and state serialization (powered by source-generated `System.Text.Json`).

- **JSVaporizer**:
  - Simplifies calls between .NET and JavaScript in WebAssembly environments by combining `[JSImport]` and `[JSExport]` into easy-to-use partial classes.  
  - Manages ephemeral `JSObject` references so you don’t have to worry about disposing them manually.  
  - Provides dictionary-based pooling for function callbacks, allowing dynamic event binding.

When fused, these libraries let you define or instantiate UI components on the fly (from JSON), manipulate DOM elements directly from C#, handle events with minimal boilerplate, and store or transform data in a structured, AOT-friendly way.

### Reduce Spaghetti

**NuFlexiArch + JSVaporizer** can be especially helpful for business applications that risk turning into spaghetti code. Here’s how:

1. **Centralized Data Flow (DTO-Centric)**  
   - Instead of sprinkling JSON parsing and data manipulation across multiple files, everything lives in **transformers** (like `JsonToDto` and `DtoToView`) and **components** (holding `CompStateDto`).  
   - You keep logic and data transformations easy to find and debug, rather than scattered in random scripts.

2. **Clean Separation of Concerns**  
   - NuFlexiArch separates **transformation logic** from **UI state** and **rendering**.  
   - This prevents the common pitfall where a single file does both raw DOM manipulation and heavy business logic, thereby improving maintainability.

3. **Reduced JavaScript Boilerplate**  
   - JSVaporizer wraps most DOM calls and event handling in C#, stopping you from writing one-off JS that quickly accumulates into a tangle.  
   - Everything is in .NET, so you get better **type checking**, **debugging**, and adherence to C# best practices (like analyzers and unit tests).

4. **Reusable, Modular Components**  
   - A component can be repurposed in multiple forms or pages without rewriting data-handling code. Just plug in different transformers or tweak the DTO.  
   - Keeping business rules consistent in a known pattern means you’re less likely to break existing code when adding new functionality.

5. **Easier Maintenance and Onboarding**  
   - New team members see a **predictable** structure: “Here’s the transformer for that data. Here’s the component that uses it.” They’re not hunting for logic in random JS snippets.  
   - If your data changes (e.g., adding a field to your business object), you know exactly which transformer or component to update.

**NuFlexiArch + JSVaporizer** helps maintain a **structured, type-safe** approach for front-end or full-stack business applications. By keeping data transformations, UI logic, and minimal JS bridging in well-defined areas, it’s harder to accidentally create a spaghetti codebase.

### Suitable for AI-assisted development

**NuFlexiArch + JSVaporizer** is particularly well-suited for AI-assisted development:

1. **Clear, Uniform Patterns**  
   - Each part of the framework (DTOs, Transformers, Components) follows predictable method signatures (e.g., `JsonToDto`, `DtoToView`, `SetState`, `GetState`).  
   - AI tools (like ChatGPT) can easily learn and replicate these patterns without needing to guess ad-hoc conventions.

2. **Separation of Responsibilities**  
   - Transformers and Components each have well-defined roles: data conversion vs. DOM/state handling.  
   - This clear separation helps AI generate code for each step without mixing concerns.

3. **Reduced JavaScript Complexity**  
   - DOM operations are in C#, thanks to JSVaporizer. No scattered JS or TypeScript is necessary.  
   - AI can produce or update C# DOM calls more consistently than dealing with multiple languages.

4. **DTO-Centric Model**  
   - Data is always in strongly typed DTOs, making it straightforward for AI to parse and manipulate.  
   - Adding fields (like `PhoneNumber`) leads to predictable boilerplate updates that AI can quickly implement.

5. **Predictable Lifecycle Methods**  
   - Known patterns (e.g., `JsonToDto`, `DtoToView`) let AI generate scaffolding.  
   - You can easily spot missing steps or mismatched IDs because everything follows a uniform naming scheme.

6. **Easier Code Review**  
   - Humans can more quickly verify AI-generated code when it follows a known structure.  
   - This reduces the time spent debugging or manually implementing repetitive boilerplate.

**Conclusion**: By focusing on consistent method signatures, minimal JavaScript, and a strongly typed DTO approach, **NuFlexiArch + JSVaporizer** naturally aligns with AI code-generation tools—enhancing productivity and reducing the risk of spaghetti code.


---

## Distinctive Features

### 1. Ephemeral `JSObject` Management

**JSVaporizer** automatically disposes the underlying `JSObject` each time you interact with a DOM element if it’s already connected to the DOM. This prevents memory bloat (from leftover JS objects) and lowers the risk of accidentally referencing stale DOM pointers. You get:

- **On-demand** fetch-and-dispose strategy: each call to `Document.GetElementById(...)` returns a fresh `JSObject`, which JSVaporizer disposes shortly after use.  
- **Reduced risk** of memory leaks and easier debugging of interop code.

### 2. Dictionary-Based Callback Pools

Rather than requiring a dedicated `[JSExport]` method for each JavaScript callback, JSVaporizer stores delegates in dictionaries keyed by simple strings:

- **Dynamic event handler registration**: attach or detach event callbacks at runtime without generating new exported methods.  
- **Reduced clutter**: keep your code DRY by avoiding repetitious “one export per callback” patterns.  
- **Easy debugging**: track function keys in a single dictionary for clarity on which handlers are active.

### 3. NuFlexiArch Transformer Registry

**NuFlexiArch** introduces a “transformer” concept, represented by `ITransformer` and `TransformerDto`. The registry (`ITransformerRegistry`) lets you:

- **Register** multiple transformers under different keys.  
- **Invoke** them at runtime (`Invoke(...)`) to convert JSON → DTO → (optional) view string.  
- Swap or upgrade transformation logic without altering the main code, just by pointing to a different transformer key.

### 4. Reflection-Based Dynamic Component Creation

**IJSVComponent** provides a mechanism to **reflectively construct** components based on metadata (e.g., type name) and a unique prefix:

- **`InitializeFromJson(...)`** method:  
  1. Deserializes JSON metadata to determine the component’s type and prefix.  
  2. Instantiates the component with reflection.  
  3. Deserializes and applies the component’s state.  
- Ideal for building dynamic UIs that can be specified or updated purely via JSON.

### 5. Source-Generated JSON + AOT Friendliness

All major classes (`CompStateDto`, `ComponentMetadata`, etc.) are annotated with `[JsonSerializable(...)]` partial contexts:

- **Compile-time** generation of serialization code reduces reflection overhead.  
- Works well with **.NET WASM AOT** scenarios, ensuring smaller binaries and faster startup times.

### 6. Minimal, Modular DOM Wrapping

JSVaporizer doesn’t attempt to replicate the entire DOM or create a full component framework. Instead, it offers:

- **`Document`** and **`Element`** classes for essential DOM operations (`AppendChild`, `SetProperty`, `GetAttribute`, etc.).  
- **Event Hooks** via `AddEventListener`, which ties in with the dictionary-based delegate pool.

This is perfect if you want a light approach to raw DOM control rather than adopting a comprehensive framework like Blazor or Angular.

### 7. Two-Fold Architecture for UI + Data

- **NuFlexiArch**: Helps define a “data-driven UI” approach where each component has state (DTO) and metadata.  
- **JSVaporizer**: Provides the actual interop channels for updating DOM elements, hooking up event handlers, or calling JS functions from .NET.  

Together, they streamline end-to-end flows: *json* → *component instantiation* → *render DOM* → *manipulate DOM / handle events in .NET* → *synchronize state*.

---

## High-Level Workflow

1. **Register Transformers**: In your .NET code, populate the `TransformerRegistry` with one or more **`JSVTransformer`** subclasses.  
2. **Create/Lookup Components**: Either create an `AComponent` subclass (e.g., `JSVTextInput`) directly or reflect it using `InitializeFromJson(...)`.  
3. **Render (Optional)**: If server-side HTML generation is used, call your `IComponentRenderer` to produce markup.  
4. **DOM Interop**: On the client side, rely on JSVaporizer’s `Document` and `Element` wrappers to set properties, attach event listeners, etc.  
5. **Call Transformers**: If you need to transform data (like a DTO) to a “view” string or back, invoke the registry’s `DtoToView()` or `JsonToDto()` methods.  
6. **Handle State**: Store, serialize, or reload component state as needed, always referencing the source-generated JSON contexts for performance.

---

## Usage Examples

### Registering a Transformer

```csharp
var registry = new TransformerRegistry(new Dictionary<string, JSVTransformer>
{
    ["myKey"] = new MyCustomTransformer(),
    ["anotherKey"] = new AnotherTransformer()
});

// Now registry.Get("myKey") gives you MyCustomTransformer
```

### Invoking a Transformer

```csharp
string dtoJson = "...";
string userInfoJson = "...";
string htmlView = TransformerRegistry.Invoke(registry, "myKey", dtoJson, userInfoJson);
// Produces a view string via DtoToView
```

### Creating a Component Programmatically

```csharp
var textInput = new JSVTextInput("TextInputPrefix", new TextInputRenderer());
var stateDto = new TextInputStateDto { LabelValue = "Hello", InputValue = "World" };
textInput.SetState(stateDto);

// If you need server-side rendering:
await textInput.GetRenderer().RenderAsync(textInput, htmlHelper, new HtmlContentBuilder());
```

### Initializing a Component from JSON (Reflection)

```csharp
// Suppose you have metadataJson and stateDtoJson from somewhere
bool success = IJSVComponent.InitializeFromJson(metadataJson, stateDtoJson);
```

Or from JavaScript (if `[JSExport]` is used):

```csharp
Module.JSVComponentInitializer.InitializeFromJson(metadataJson, stateDtoJson);
```
### Project Structure

```csharp
NuFlexiArch/
├── ITransformer.cs, TransformerDto.cs, ITransformerRegistry.cs
├── IComponent.cs, AComponent.cs, CompStateDto.cs, ...
├── <JsonSerializerContext classes>
└── ...

JSVNuFlexiArch/
├── JSVTransformer.cs
├── TransformerRegistry.cs
├── IJSVComponent.cs, JSVComponentRenderer.cs
└── ...

MyViewLib/
├── JSVComponentInitializer.cs
├── ATextInput.cs, JSVTextInput.cs, TextInputRenderer.cs
└── ...

JSVaporizer/
├── JSVapor.cs (public static partial classes for Imports/Exports)
├── Element.cs
├── Document.cs
├── <Other partial classes>
└── ...
```
