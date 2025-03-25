# JSVaporizer

**Cut down on JavaScript. Write more C#.**  
A minimalistic .NET 8 WebAssembly library that helps you unify front-end and back-end logic without committing to a full framework like Blazor.

---

## Why JSVaporizer?

Most ASP.NET Core apps need some front-end interactivity: DOM manipulation, event handlers, and data transformation. But juggling both JavaScript and C# can be messy:

- You end up writing repetitive JavaScript and JQuery boilerplate everywhere.
- Business logic is split across two languages, and you have to maintain it in two languages.
- Front-end code can become spaghetti.

**JSVaporizer** offers a **middle ground**:

- It’s lighter than a full SPA framework (like Blazor).
- It cuts down on JavaScript by letting you write front-end logic in C#.
   - Use the same business logic source code in both the front and the back ends.
- You're not boxed in. You can still add custom JavaScript if you choose.

---

## JSVaporizer Assembly

1. **DOM Facade in C#**  
   Typical DOM calls like `getElementById`, `setAttribute`, or `addEventListener` are wrapped in easy C# methods. No more copy-pasting JavaScript for every form or event handler.

2. **Minimal JavaScript**  
   A small script loads the .NET WASM runtime (which isn't tiny), then delegates all serious logic (event handling, validation, data manipulation) to C#. You can still drop in small custom JS for specialized tasks—no big frameworks needed.

3. **Extensible in Four Ways**  
   - **Call arbitrary JS** from C#: If you need a special function or a quick fix, register it in JSVaporizer’s function pool.  
   - **Call arbitrary C#** from JS (whitelisted methods): Perfect for callbacks or event hooks.  
   - **Import your own JS** modules via `[JSImport]`.  
   - **Export your own C#** methods to JavaScript via `[JSExport]`.

---

## JSVTransformer Extension

1. **Define a Transformer**  
   Implement `ITransformer` or extend the `Transformer` base class. Implement methods like `DtoToView()`, which updates DOM fields from your C# DTO, and `ViewToDto()`, which pulls form data back into the DTO.

2. **Register the Transformer**  
   Make it discoverable via a registry key (e.g., `"MyCoolTransformer"`), so JavaScript can call it by name. Or use any other approach (like a dictionary) to store your transformers.

3. **Use Minimal JavaScript**  
   In your page’s JS, just import the WASM and call something like  
   ```js
   TransformerInvoker.Invoke("MyCoolTransformer", dtoJSON);
   ```

