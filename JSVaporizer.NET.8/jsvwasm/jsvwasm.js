"use strict";

export let jsvExportConfig;
export let exports;

import { dotnet } from '../_framework/dotnet.js'
import * as ImportsForDotNet from './for_dotnet/_for_dotnet.js';
import { registerJSFunction } from './for_dotnet/js_function_pool.js';

const { setModuleImports, getAssemblyExports, getConfig } = await dotnet
    .withDiagnosticTracing(false)
    .withApplicationArgumentsFromQuery()
    .create();

// Give this to JavaScript.
jsvExportConfig = getConfig();

// Get this stuff from C#
let jsvExports = await getAssemblyExports("JSVaporizer.NET.8");

// Give this stuff to C#
let forDotnet = ImportsForDotNet.getForDotNet(jsvExports.JSVaporizer.JSVapor.WasmExports);
setModuleImports('element', forDotnet.element);
setModuleImports('document', forDotnet.document);
setModuleImports('window', forDotnet.window);
setModuleImports('jsFunctionPool', forDotnet.jsFunctionPool);

export function jsvRegisterCustomImports(importKey, zCustomImports) {
    setModuleImports(importKey, zCustomImports);
}

export function jsvRegisterJSFunction(funcKey, func) {
    registerJSFunction(funcKey, func);
}

export function callJSVGenericFunction(funcKey, ...args) {
    jsvExports.JSVapor.WasmExports.CallJSVGenericFunction(funcKey, args);
}

export async function GetExportedAssembly(name) {
    let assm = await getAssemblyExports(name);
    return assm;
}

