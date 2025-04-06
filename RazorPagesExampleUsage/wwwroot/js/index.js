"use strict";

let site;

import("./site.js").then((module) => {
    site = module;

    // Launch your front end here
    LaunchApp();
});

async function LaunchApp() {
    let JsvWasm = await site.GetJsvWasm();

    // Get exports from any web assemblies exported.
    let jsvExports = await JsvWasm.GetExportedAssembly("MyViewLib");

    let myTextInput_MetadataJson = $("#hf_MyTextInput_MetadataJson").val();
    let myTextInput_StateDtoJson = $("#hf_MyTextInput_StateDtoJson").val();

    let resStr = jsvExports.MyViewLib.JSVComponentInitializer.InitializeFromJson(myTextInput_MetadataJson, myTextInput_StateDtoJson);

    alert("MyTextInput says: " + resStr);
}


