"use strict";

let jsvExports;
let jsvRegisterCustomImports;
let jsvRegisterJSFunction;
let callJSVGenericFunction;

import("../jsvwasm/jsvwasm.js").then((jsvWasm) => {
    jsvExports = jsvWasm.jsvExports;
    jsvRegisterCustomImports = jsvWasm.jsvRegisterCustomImports;
    jsvRegisterJSFunction = jsvWasm.jsvRegisterJSFunction;
    callJSVGenericFunction = jsvWasm.callJSVGenericFunction;

    // Launch your front end here
    doCoolThings();
});

function doCoolThings() {

    // Register any (hopefully small!) one-off JS functions you need to.
    // Maybe you need to do this for a quick fix.
    // Don't forget: Quick fixes have a strange way of becoming permanent spaghetti code.
    //
    // Remember: The goal is to REDUCE the amount of BS JS.

    jsvRegisterJSFunction("AjaxPOST", AjaxPOST);

    let dtoJSON = $("#hfDtoJSON").val();

    let resStr = jsvExports.TransformerInvoker.Invoke("MyCoolTransformerV1", dtoJSON);

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
            callJSVGenericFunction(successFuncKey, result);
        },
        error: function (err) {
            callJSVGenericFunction(errorFuncKey, err);
        }
    });
}
