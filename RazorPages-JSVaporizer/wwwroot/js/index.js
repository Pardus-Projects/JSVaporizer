"use strict";

let jsvExportConfig;
let jsvExports;
let jsvRegisterCustomImports;
let jsvRegisterJSFunction;
let jsvCallJSVGenericFunction;
let jsvGetExportedAssembly;

import("../jsvwasm.js").then((jsvWasm) => {
    jsvExportConfig = jsvWasm.jsvExportConfig;
    jsvRegisterCustomImports = jsvWasm.jsvRegisterCustomImports;
    jsvRegisterJSFunction = jsvWasm.jsvRegisterJSFunction;
    jsvCallJSVGenericFunction = jsvWasm.callJSVGenericFunction;
    jsvGetExportedAssembly = jsvWasm.GetExportedAssembly;

    // Launch your front end here
    doCoolThings();
});

async function doCoolThings() {

    // Register any (hopefully small!) one-off JS functions you need to.
    // Maybe you need to do this for a quick fix.
    // Don't forget: Quick fixes have a strange way of becoming permanent spaghetti code.
    //
    // Remember: The goal is to REDUCE the amount of BS JS.

    jsvRegisterJSFunction("AjaxPOST", AjaxPOST);

    let dtoJSON = $("#hfDtoJSON").val();

    jsvExports = await jsvGetExportedAssembly("MyTransformerLib");

    let resStr = jsvExports.MyTransformerLib.MyTransformerRegistry.Invoke("MyCoolTransformerV1", dtoJSON);

    alert(resStr);
}

function AjaxPOST(url, dtoJSON, successFuncKey, errorFuncKey) {
    var payload = new FormData();
    payload.append("dtoJSON", dtoJSON);

    $.ajax({
        type: "POST",
        url: url,
        contentType: false,
        processData: false,
        data: payload,
        success: function (result) {
            jsvCallJSVGenericFunction(successFuncKey, result);
        },
        error: function (err) {
            jsvCallJSVGenericFunction(errorFuncKey, err);
        }
    });
}
