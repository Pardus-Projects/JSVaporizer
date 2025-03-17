import { jsvExports } from "./jsvwasm/jsvwasm.js";

// Now do cool things
let resStr = jsvExports.Test.InvokeTest("TESTING");

//alert(resStr);
console.log(resStr);