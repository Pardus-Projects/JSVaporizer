"use strict";

let site;

import("./site.js").then((module) => {
    site = module;

    // Launch your front end here
    LaunchApp();
});

async function LaunchApp() {
    let JsvWasm = await site.GetJsvWasm();

    // Register any JS functions you want C# to see.
    JsvWasm.RegisterJSFunction("AjaxPOST", site.AjaxPOST);

    // Get exports from any web assemblies exported.
    let jsvExports = await JsvWasm.GetExportedAssembly("JSVaporizer.NET.8");

    let barberTransformerAQN = $("#hfXFormerAQN").val();
    let barberTransformerDtoJSON = $("#hfDtoJSON").val();
    let resStr = jsvExports.JSVNuFlexiArch.JSVTransformerInvoker.Invoke(barberTransformerAQN, barberTransformerDtoJSON);
    alert("MyCoolTransformerV1 says: " + resStr);
}

