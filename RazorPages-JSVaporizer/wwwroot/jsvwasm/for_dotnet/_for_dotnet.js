"use strict";

import * as element from './element.js';
import * as document from './document.js';
import * as window from './window.js';
import * as jsFunctionPool from './js_function_pool.js';

export function getForDotNet(exports) {
    return {
        element: element.getElement(exports),
        document: document.getDocument(exports),
        window: window.getWindow(exports),
        jsFunctionPool: jsFunctionPool.getJSFunctionPool(exports),
    };
}



