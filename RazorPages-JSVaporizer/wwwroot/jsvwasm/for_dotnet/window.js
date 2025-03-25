"use strict";

let jsvExports;

export function getWindow(exports) {
    jsvExports = exports;
    return {
        alert: (s) => alert(s),
        location: {
            href: () => window.location.href
        },
        console: {
            log: (str) => console.log(str),
            dir: (obj) => console.dir(obj)
        }
    };
}
