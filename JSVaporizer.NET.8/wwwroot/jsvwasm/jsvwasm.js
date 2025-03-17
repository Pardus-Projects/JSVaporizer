"use strict";

export let jsvExports;

import { dotnet } from '../_framework/dotnet.js'
import * as ImportsForDotNet from './for_dotnet/_for_dotnet.js';
import { registerJSFunction } from './for_dotnet/js_function_pool.js';

const { setModuleImports, getAssemblyExports, getConfig } = await dotnet
    .withDiagnosticTracing(false)
    .withApplicationArgumentsFromQuery()
    .create();

// Get this stuff from C#
const config = getConfig();
const exports = await getAssemblyExports(config.mainAssemblyName);

jsvExports = exports.JSVaporizer;

// Give this stuff to C#
let forDotnet = ImportsForDotNet.getForDotNet(jsvExports.JSVapor.WasmExports);
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

