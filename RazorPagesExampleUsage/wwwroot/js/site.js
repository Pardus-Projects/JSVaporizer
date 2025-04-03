"use strict";

let JsvWasm = null;

export async function GetJsvWasm() {
    if (JsvWasm == null) {
        await import("../jsvwasm.js").then((jsvWasm) => {
            JsvWasm = {
                ExportConfig: jsvWasm.jsvExportConfig,
                RegisterCustomImports: jsvWasm.jsvRegisterCustomImports,
                RegisterJSFunction: jsvWasm.jsvRegisterJSFunction,
                CallJSVGenericFunction: jsvWasm.callJSVGenericFunction,
                GetExportedAssembly: jsvWasm.GetExportedAssembly
            };
        });
    }

    return JsvWasm;
}

export function AjaxPOST(url, dtoJSON, successFuncKey, errorFuncKey) {
    GetJsvWasm();

    var payload = new FormData();
    payload.append("dtoJSON", dtoJSON);

    $.ajax({
        type: "POST",
        url: url,
        contentType: false,
        processData: false,
        data: payload,
        success: function (result) {
            JsvWasm.CallJSVGenericFunction(successFuncKey, result);
        },
        error: function (err) {
            JsvWasm.CallJSVGenericFunction(errorFuncKey, err);
        }
    });
}
