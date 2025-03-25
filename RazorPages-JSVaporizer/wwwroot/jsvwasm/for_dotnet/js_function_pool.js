"use strict";

let jsvExports;

export function getJSFunctionPool(exports) {
    jsvExports = exports;
    return {

        callJSFunction: (funcKey, args) => callJSFunction(funcKey, args),

    };
}

let jsFunctionPool = {};

export function registerJSFunction(funcKey, func) {
    jsFunctionPool[funcKey] = func;
}

function callJSFunction(funcKey, args) {
    jsFunctionPool[funcKey](...args);
}
