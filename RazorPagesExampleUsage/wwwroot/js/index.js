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

    let resStr;

    let JSVTextDisplay_InstanceJson = $("#hf_JSVTextDisplay_InstanceJson").val();
    resStr = jsvExports.MyViewLib.JSVComponentInitializer.InstantiateFromJson(JSVTextDisplay_InstanceJson, "JSVTextDisplay_Placeholder");
    alert("JSVTextDisplay says: " + resStr);

    let JSVTextInput_InstanceJson = $("#hf_JSVTextInput_InstanceJson").val();
    resStr = jsvExports.MyViewLib.JSVComponentInitializer.InstantiateFromJson(JSVTextInput_InstanceJson, "JSVTextInput_Placeholder");
    alert("JSVTextInput says: " + resStr);

    let JSVTextArea_InstanceJson = $("#hf_JSVTextArea_InstanceJson").val();
    resStr = jsvExports.MyViewLib.JSVComponentInitializer.InstantiateFromJson(JSVTextArea_InstanceJson, "JSVTextArea_Placeholder");
    alert("JSVTextArea says: " + resStr);

    let JSVCheckbox_InstanceJson = $("#hf_JSVCheckbox_InstanceJson").val();
    resStr = jsvExports.MyViewLib.JSVComponentInitializer.InstantiateFromJson(JSVCheckbox_InstanceJson, "JSVCheckbox_Placeholder");
    alert("JSVCheckbox says: " + resStr);

    let JSVSlider_InstanceJson = $("#hf_JSVSlider_InstanceJson").val();
    resStr = jsvExports.MyViewLib.JSVComponentInitializer.InstantiateFromJson(JSVSlider_InstanceJson, "JSVSlider_Placeholder");
    alert("JSVSlider says: " + resStr);
}


