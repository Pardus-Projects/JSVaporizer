# JSVaporizer

**Cut down on JavaScript. Write more C#.**  
A minimalistic .NET 8 WebAssembly library that lets you unify front-end and back-end logic without committing to a full framework like Blazor.

---

## Why JSVaporizer?

Most ASP.NET Core apps need some front-end interactivity: DOM manipulation, event handlers, and data transformation. But juggling both JavaScript and C# can be messy:

- You end up writing repetitive JavaScript and JQuery boilerplate everywhere.
- Business logic is split across two languages, and you have to maintain it in two languages.
- Front-end code can become spaghetti.

**JSVaporizer** offers a **middle ground**:

- It’s lighter than a full SPA framework (like Blazor).
- It cuts down on JavaScript by letting you write front-end logic in C# — no major rewrites needed.
- It doesn’t lock you into any specific ecosystem. You can still add custom JavaScript or even build a Blazor-style persistent connection if you want.

---

## Core Ideas

1. **DTO-Centric**  
   Define a **DTO** (Data Transfer Object) in C#. Transform it to (and from) the DOM using a **Transformer**. Keep your domain logic strongly typed and maintainable.

2. **DOM Facade in C#**  
   Typical DOM calls like `getElementById`, `setAttribute`, or `addEventListener` are wrapped in easy C# methods. No more copy-pasting JavaScript for every form or event handler.

3. **Minimal JavaScript**  
   A tiny script loads the .NET WASM runtime, then delegates all serious logic (event handling, validation, data manipulation) to C#. You can still drop in small custom JS for specialized tasks—no big frameworks needed.

4. **Extensible in Four Ways**  
   - **Call arbitrary JS** from C#: If you need a special function or a quick fix, register it in JSVaporizer’s function pool.  
   - **Call arbitrary C#** from JS (whitelisted methods): Perfect for callbacks or event hooks.  
   - **Import your own JS** modules via `[JSImport]`.  
   - **Export your own C#** methods to JavaScript via `[JSExport]`.

5. **Works With .NET 8**  
   Leverages the new [Microsoft.NET.Runtime.WebAssembly.Sdk](https://www.nuget.org/packages/Microsoft.NET.Runtime.WebAssembly.Sdk) and [.NET 8 JavaScript Interop APIs](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.javascript?view=net-8.0). Future .NET releases will likely make this story even stronger.

---

## How It Works

1. **Define a Transformer**  
   Implement `ITransformer` or extend the `Transformer` base class. Implement methods like `DtoToView()`, which updates DOM fields from your C# DTO, and `ViewToDto()`, which pulls form data back into the DTO.

2. **Register the Transformer**  
   Make it discoverable via a registry key (e.g., `"MyCoolTransformer"`), so JavaScript can call it by name. Or use any other approach (like a dictionary) to store your transformers.

3. **Use Minimal JavaScript**  
   In your page’s JS, just import the WASM and call something like  
   ```js
   TransformerInvoker.Invoke("MyCoolTransformer", dtoJSON);
   ```

That’s it: data is transformed, the DOM is updated, event handlers are attached—all in C#.

## Add Custom Logic Where Needed
If you need extra JavaScript calls, register them in JSVaporizer’s `JSFunctionPool`.  
If you want to push data from JavaScript to C#, whitelisting a C# method is just as easy.

---

## Real-World Scenario

### Forms and Validation
Let’s say you have a form with multiple fields and a “Submit” button. Instead of writing form-filling code in JS, you create a C# DTO (like `AppointmentDto`), then a `Transformer` that:

1. Reads the `AppointmentDto` JSON and populates the form (`DtoToView`).
2. Hooks the button’s event listener in C#.
3. On click, calls `ViewToDto()`, validates the data, and maybe sends it to your server.

### AJAX or Real-Time Calls
You can still do AJAX from minimal JS or adopt a more advanced persistent connection—JSVaporizer doesn’t restrict you. Want to do a quick `fetch` or jQuery call? Just drop in a small JS snippet and call it from C#.

---

## Who Is It For?
- .NET Devs who want to unify front-end/back-end logic without going “all-in” on Blazor.  
- Teams who hate duplicating logic in JavaScript and prefer static typing and .NET tooling.  
- Projects that aren’t full SPAs but still need some front-end interactivity.

---

## Who Is It Not For?
- Those heavily invested in SPA frameworks like React, Angular, or Vue.  
- Developers who want a full Blazor experience (component model, two-way binding, etc.).  
- Teams comfortable with polyglot front ends who don’t mind more JavaScript.

---

2025
