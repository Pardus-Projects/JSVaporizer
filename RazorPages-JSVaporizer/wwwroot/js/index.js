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

    // Register any JS functions you want C# to see.
    jsvRegisterJSFunction("AjaxPOST", AjaxPOST);

    // Get exports from any web assemblies exported.
    jsvExports = await jsvGetExportedAssembly("MyTransformerLib");

    let coolTransformerDtoJSON = $("#hfDtoJSON").val();
    let resStr = jsvExports.MyTransformerLib.MyTransformerRegistry.Invoke("MyCoolTransformerV1", coolTransformerDtoJSON);
    alert("MyCoolTransformerV1 says: " + resStr);


    let exampleComponentDtoJson = $("#hfExampleComp_DtoJson").val();
    let compInfoJson = $("#hfExampleComp_CompInfoJson").val();
    resStr = jsvExports.MyTransformerLib.MyTransformerRegistry.Invoke("MyExampleCompTransformer", exampleComponentDtoJson, compInfoJson);
    alert("MyExampleCompTransformer says: " + resStr);
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
