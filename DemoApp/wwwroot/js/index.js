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
    let jsvExports = await JsvWasm.GetExportedAssembly("JSVaporizer.NET.8");

    let MyTestCompBuilder_UN = $("#hf_MyTestCompBuilder_UN").val();
    let MyTestCompBuilder_AQN = $("#hf_MyTestCompBuilder_AQN").val();
    await jsvExports.JSVNuFlexiArch.JSVCompBuilderInvoker.Invoke(MyTestCompBuilder_UN, MyTestCompBuilder_AQN, "MyTestComp_Placeholder");

    let RegistrationFormComp_UN = $("#hf_RegistrationFormComp_UN").val();
    let RegistrationFormComp_AQN = $("#hf_RegistrationFormComp_AQN").val();
    await jsvExports.JSVNuFlexiArch.JSVCompBuilderInvoker.Invoke(RegistrationFormComp_UN, RegistrationFormComp_AQN, "RegistrationFormComp_Placeholder");

    alert("FINISHED");
}


