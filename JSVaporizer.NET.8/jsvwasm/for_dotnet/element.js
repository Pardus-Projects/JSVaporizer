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

function getPropertyNamesArray(elem) {
    var props = [];
    for (var key in elem) {
        props.push(key);
    }
    return props;
}

// id -> listener fn
const eventListenerFuncSpace = new Map();               // Map<int, Function>

// element -> Set<ids>
const eventListenerElementIds = new WeakMap();          // WeakMap<Element, Set<int>

function addEventListener(elem, eventType, listenerId) {
    if (eventListenerFuncSpace.has(listenerId))
        throw new Error(`Listener id ${listenerId} is already registered.`);

    let eventListener = function (event) {
        let behaviorMode = jsvExports.CallJSVEventListener(listenerId, elem, eventType, event);

        // behaviorMode = 0 : preventDefault = false, stopPropagation = false
        // behaviorMode = 1 : preventDefault = false, stopPropagation = true
        // behaviorMode = 2 : preventDefault = true, stopPropagation = false
        // behaviorMode = 3 : preventDefault = true, stopPropagation = true

        const preventDefault = behaviorMode == 2 || behaviorMode == 3;
        const stopPropagation = behaviorMode == 1 || behaviorMode == 3;
        if (preventDefault) {
            event.preventDefault();
        }
        if (stopPropagation) {
            event.stopPropagation();
        }
    };
    eventListenerFuncSpace.set(listenerId, eventListener);
    elem.addEventListener(eventType, eventListener);

    if (!eventListenerElementIds.has(elem)) {
        eventListenerElementIds.set(elem, new Set());
    }
    eventListenerElementIds.get(elem).add(listenerId);

    return eventListenerFuncSpace.size;
}

function removeEventListener(elem, eventType, listenerId) {
    const listener = eventListenerFuncSpace.get(listenerId);
    if (listener) {
        elem.removeEventListener(eventType, listener);
        eventListenerFuncSpace.delete(listenerId);

        const set = eventListenerElementIds.get(elem);
        if (set) {
            set.delete(listenerId);
            if (set.size === 0) {
                eventListenerElementIds.delete(elem);
            }
        }
    }

    return eventListenerFuncSpace.size;
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
