
# ZenArch

**A unified, data-centric .NET WebAssembly framework to tame large-scale front-end complexity.**  
Derived from the fusion of **NuFlexiArch** (transformers + components) and **JSVaporizer** (minimal .NET↔JS interop), **ZenArch** is designed to help teams avoid spaghetti code, reduce redundant JavaScript, and scale cleanly across big business applications or distributed microservice architectures.

---

## Table of Contents

1. [Overview](#overview)  
2. [Key Goals](#key-goals)  
3. [Why ZenArch?](#why-zenarc)  
4. [Getting Started](#getting-started)  
5. [Core Concepts](#core-concepts)  
   1. [DTO-Centric Architecture](#dto-centric-architecture)  
   2. [NuFlexiArch Components & Transformers](#nuflexiarch-components--transformers)  
   3. [JSVaporizer DOM Interop](#jsvaporizer-dom-interop)  
6. [Sample Code Snippet](#sample-code-snippet)  
7. [Use Cases](#use-cases)  
8. [Advanced Topics](#advanced-topics)  
   1. [AI-Assisted Development](#ai-assisted-development)  
   2. [Multi-Service or Microservice Workflows](#multi-service-or-microservice-workflows)  
   3. [Handling Complex Forms](#handling-complex-forms)  
9. [Roadmap](#roadmap)  
10. [Contributing](#contributing)  
11. [License](#license)

---

## Overview

**ZenArch** combines:
- **NuFlexiArch**: A flexible, DTO-centric architecture that uses “components” and “transformers” to structure your application’s data flow.  
- **JSVaporizer**: A lightweight .NET-to-JavaScript interop layer that drastically reduces the need for raw JavaScript, exposing DOM actions and event handling in a clear, minimal API.

This synergy provides a **structured, scalable** foundation to manage complex front-end logic for **large-scale business apps** or distributed systems. Using **ZenArch**, teams can keep code modular, minimize debugging overhead, and consistently handle data transformations from server to UI.

---

## Key Goals

1. **Eliminate Spaghetti Code**  
   - Ensure that even when an application grows to hundreds of modules or microservices, front-end code remains predictable and maintainable.

2. **DTO-Centric Data Flow**  
   - Keep data transformations explicit. All JSON ↔ application state ↔ UI transitions happen in well-defined “transformer” methods.

3. **Minimal JavaScript**  
   - Avoid scattering JS across the project. Use ephemeral `JSObject`s and dictionary-based event delegation so C# handles DOM and events directly.

4. **Scalable for Teams**  
   - Encourage a **common pattern** so new developers easily onboard, find relevant code, and follow consistent method signatures (`JsonToDto`, `DtoToView`, etc.).

5. **AI-Assisted Development**  
   - Provide clear patterns that AI tools (like ChatGPT or Copilot) can grasp, generating or refactoring code in a straightforward, minimal-boilerplate manner.

---

## Why ZenArch?

- **Structured Architecture**: By merging data transformations (NuFlexiArch) with a minimal interop layer (JSVaporizer), you achieve a system designed for large codebases—no need for a monolithic front-end.
- **AOT Friendly**: Built on .NET WASM technology; source-generated JSON contexts help with performance and smaller WASM footprints.
- **Clean Separation**: Components handle UI state, Transformers handle data logic, DOM calls remain in C#—makes for a more maintainable codebase.
- **Ideal for Distributed Systems**: Integrates easily with microservices that pass JSON; simply feed the JSON into a transformer to update your front-end logic or UI.

---

## Core Concepts

### DTO-Centric Architecture

At the heart of **ZenArch** is the idea that **data** flows through discrete objects (DTOs) rather than scattered variables. By converting to and from JSON, these DTOs are easily transported between microservices or stored for offline scenarios.

### NuFlexiArch Components & Transformers

1. **Components**  
   - Subclass `AComponent` or implement `IComponent`. This is where you define how your “module” or “feature” organizes data (`SetState`, `GetState`) and, optionally, how it “renders” (if it needs to show a UI).

2. **Transformers**  
   - Classes that implement methods like `JsonToDto` (load data from JSON), `DtoToView` (write data to the DOM), `ViewToDto` (read DOM back to data), and `DtoToJson` (serialize data).  
   - Each transformer can be **registered** in a `TransformerRegistry` under a unique string key, making it easy to swap or update transformations without large code changes.

### JSVaporizer DOM Interop

- **Ephemeral `JSObject`s**: Every time you call `Document.AssertGetElementById(...)`, you get a short-lived handle disposed after usage—so you avoid memory leaks or stale references.  
- **Dictionary-Based Events**: Instead of multiple `[JSExport]` handlers, store them in a dictionary keyed by a string. JavaScript can invoke them by calling `WasmExports.CallJSVEventHandler("myEventKey", ...)`.  
- **Minimal JS**: In many cases, you only need small stubs on the JS side—like an `AjaxPOST` function—while all front-end logic and event wiring remains in your .NET code.

## Use Cases

- **Enterprise Forms**: Large forms with multiple steps, validations, and dynamic data from microservices.  
- **IoT Dashboards**: Transform sensor data (JSON) into DOM updates, allowing real-time or event-driven UI changes with minimal JS.  
- **Multi-Step Wizards**: Keep each step as a component, store or reuse partial data in DTOs, and easily re-sequence steps as business rules change.  
- **Collaborative Apps**: Manage multiple user inputs via distributed JSON updates (like a collaborative whiteboard or doc editor).  
- **AI or External Service Calls**: Read data from an AI or external service (JSON), transform it for display, attach event handlers in .NET code, and push updates back.

---

## Advanced Topics

### AI-Assisted Development

ZenArc’s **regular, consistent** architecture is perfect for AI code generation:
- Predictable method signatures: `JsonToDto`, `DtoToView`, `ViewToDto`, etc.  
- Minimal JavaScript means an AI assistant can focus on generating or refactoring C# logic for data transformations or event handling.

### Multi-Service or Microservice Workflows

- Bring in **JSON** from multiple microservices → feed into transformers → unify the data in a single front end.  
- Each microservice’s schema can have a distinct “transformer” mapping fields to UI states, lowering integration friction.

### Handling Complex Forms

- Nest multiple “sub-components,” each with its own `CompStateDto`.  
- Compose them in a main “container” transformer or registry, letting you reorganize or reuse sub-components for different form flows.

---

## Roadmap

- **Performance Tuning** for large forms or data arrays.  
- **Additional Transformer Patterns** for partial or field-level updates.  
- **Deeper AI Integration**: Potential advanced code generation scripts for new fields or new event logic.  
- **Plugin Architecture**: A more formal plugin model for specialized transformers (e.g., file upload, chat integration).

---

## Contributing

We welcome PRs for:
1. **Bug fixes** or feature requests in the DOM wrapper logic.  
2. **New examples** or specialized transformers (like advanced validations, dynamic forms, wizards).  
3. **Documentation** improvements, tutorials, or real-world case studies showing how large teams adopt ZenArch.

1. **Fork** the repo,  
2. **Create** a feature branch,  
3. **Submit** a pull request describing your changes.


