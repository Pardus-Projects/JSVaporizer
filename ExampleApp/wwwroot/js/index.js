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
    let jsvExports = await JsvWasm.GetExportedAssembly("ExampleViewLib");
    let myCompMaterializer = jsvExports.ExampleViewLib.MyComponentMaterializer.MaterializeFromJson;

    let resStr;

    resStr = myCompMaterializer($("#hf_TextInput_Server_MetadataJson").val(), "TextInput_Wrapper");

    resStr = myCompMaterializer($("#hf_TextInput_Client_MetadataJson").val(), "TextInput_Wrapper");

    resStr = myCompMaterializer($("#hf_TwoTextInputs_MetadataJson").val(), "TwoTextInputs_Wrapper");

    resStr = myCompMaterializer($("#hf_ThreeTextInputsHandlebars_MetadataJson").val(), "ThreeTextInputsHandlebars_Wrapper");
    
    alert("resStr says: " + resStr);
}


