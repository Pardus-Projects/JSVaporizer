"use strict";

let jsvExports;

export function getDocument(exports) {
    jsvExports = exports;
    return {
        createJSVaporizerElement: (id, tagName, createdByJSVaporizerAttributeName) => createJSVaporizerElement(id, tagName, createdByJSVaporizerAttributeName),
        getElementById: (id) => document.getElementById(id),
        getElementsArrayByTagName: (tagName) => getElementsArrayByTagName(tagName)
    };
}

function createJSVaporizerElement(id, tagName, createdByJSVaporizerAttributeName) {
    let elem = document.createElement(tagName);
    elem.setAttribute("id", id);
    elem.setAttribute(createdByJSVaporizerAttributeName, "yes");

    return elem;
};

function getElementsArrayByTagName(tagName) {
    // Note:
    //      See:
    //          https://developer.mozilla.org/en-US/docs/Web/API/Document/getElementsByTagName
    //          https://developer.mozilla.org/en-US/docs/Web/API/HTMLCollection
    //      The issue is that
    //          document.getElementsByTagName() returns an HTMLCollection.
    //      But dotnet-interop (System.Runtime.InteropServices.JavaScript) isn't cool with that type.
    //      The solution appears to be converting HTMLCollection into an array before returning to C#.

    let htmlCollection = document.getElementsByTagName(tagName);
    let elemsFound = [];

    for (let ii = 0; ii < htmlCollection.length; ii++) {
        elemsFound.push(htmlCollection.item(ii));
    }

    return elemsFound;
}

