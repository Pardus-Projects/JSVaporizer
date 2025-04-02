"use strict";

let jsvExports;

export function getElement(exports) {
    jsvExports = exports;
    return {

        // Properties
        getPropertyNamesArray: (elem) => getPropertyNamesArray(elem),
        invokeFunctionProperty: (elem, funcPropName, args) => invokeFunctionProperty(elem, funcPropName, args),

        // Events
        addEventListener: (elem, eventType, funcKey) => addEventListener(elem, eventType, funcKey),
        removeEventListener: (elem, eventType, funcKey) => removeEventListener(elem, eventType, funcKey),
        appendChild: (elem, childElem) => elem.appendChild(childElem),

        // Attributes
        hasAttribute: (elem, attrName) => elem.hasAttribute(attrName),
        getAttribute: (elem, attrName) => elem.getAttribute(attrName),
        setAttribute: (elem, attrName, attrValue) => elem.setAttribute(attrName, attrValue),

        // HTMLOptionsCollection convenience
        getMultiSelectOptionValues: (elem) => getMultiSelectOptionValues(elem),

    };
}

let eventHandlerFuncSpace = {};

function getPropertyNamesArray(elem) {
    var props = [];
    for (var key in elem) {
        props.push(key);
    }
    return props;
}

function addEventListener(elem, eventType, funcKey) {
    if (!eventHandlerFuncSpace[funcKey]) {
        let eventHandler = function (event) {
            let behaviorMode = jsvExports.CallJSVEventHandler(funcKey, elem, eventType, event);

            // behaviorMode = 0 : preventDefault = false, stopPropagation = false
            // behaviorMode = 1 : preventDefault = false, stopPropagation = true
            // behaviorMode = 2 : preventDefault = true, stopPropagation = false
            // behaviorMode = 3 : preventDefault = true, stopPropagation = true

            let preventDefault = behaviorMode == 2 || behaviorMode == 3;
            let stopPropagation = behaviorMode == 1 || behaviorMode == 3;

            if (preventDefault) {
                event.preventDefault();
            }
            if (stopPropagation) {
                event.stopPropagation();
            }
        };
        eventHandlerFuncSpace[funcKey] = eventHandler;
        elem.addEventListener(eventType, eventHandler);
        return true;
    } else {
        throw new Error("You currently cannot use the same key value for different handlers, or to apply the same hander to multiple elements. It must be removed before it can be added again.");
        return fasle;
    }
}

function removeEventListener(elem, eventType, funcKey) {
    let eventHandler = eventHandlerFuncSpace[funcKey];
    elem.removeEventListener(eventType, eventHandler);
    delete eventHandlerFuncSpace[funcKey];

    return true;
}

function invokeFunctionProperty(elem, funcPropName, argsArray) {
    elem[funcPropName](...argsArray);
}

function getMultiSelectOptionValues(elem) {
    // Inspired by https://github.com/jquery/jquery/blob/main/src/attributes/val.js

    let options = elem.options;
    let values = [];

    // Get selected options
    for (let ii = 0; ii < options.length; ii++) {

        let option = options[ii];
        if (option.selected) {

            // See: https://github.com/jquery/jquery/blob/main/src/attributes/val.js
            if (
                !option.disabled
                && (!option.parentNode.disabled || !nodeName(option.parentNode, "optgroup"))
            ) {
                values.push(option.value);
            }
        }
    }

    return values;
}
