using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;
using static JSVaporizer.JSVapor;

namespace JSVaporizer;

internal static partial class Test
{
    [SupportedOSPlatform("browser")]
    internal static void RunTest()
    {
        Element theElem = Document.GetElementById("myButton");

        //Util.ConsoleLog(theElem);
        //Util.ConsoleDir(theElem);
        //Util.Alert("Look at dump!");

        Element myButton = Document.GetElementById("myButton");

        EventHandlerCalledFromJS clickAction = (JSObject elem, string eventType, JSObject evnt) =>
        {
            Window.Alert("eventType = " + eventType);

            myButton.SetProperty("innerHTML", "YOU CAN'T SEE ME!");

            Window.Alert("innerHTML = " + myButton.GetProperty("innerHTML").Value);

            // Remove the handler from the element.
            myButton.RemoveEventListener("click", "myButtonClick");

            return (int)JSVEventHandlerBehavior.NoDefault_NoPropagate;
        };

        // Add the handler to the element.
        myButton.AddEventListener("click", "myButtonClick", clickAction);

        //myButton.SetProperty("disabled", "true");

        Element orphanElem = Document.CreateElement("myCoolNewElement", "span");

        orphanElem.SetAttribute("attr_1", "val_1");

        Element? parentElem = Document.GetElementById("mySpan");
        if (parentElem != null)
        {
            parentElem.AppendChild(orphanElem);
        }

        orphanElem.SetProperty("prop_2", "val_2");

        var attr_1 = orphanElem.GetAttribute("attr_1");
        //object? prop_2 = orphanElem.GetProperty("attr_1");

        Document.CreateElement("theTest", "div");

        //List<string> props = orphanElem.GetPropertyNamesList();
        Dictionary<string, ElementPropInfo> propsDict = orphanElem.GetPropertiesDictionary();

        // Test converting to string
        //foreach (KeyValuePair<string, ElementPropInfo> kvp in propsDict)
        //{
        //    string propName = kvp.Key;
        //    ElementPropInfo propInfo = kvp.Value;
        //    string propType = propInfo.Type;
        //    string strVal = orphanElem.PropValToString(propName)??"<NULL>";
        //    System.Console.WriteLine($"propName = {propName}, propType = {propType}, strVal = {strVal}");
        //}
        
        string problems = Document.JSVToDomReconciliation();

        //System.Console.WriteLine(problems);

        JSVapor.Console.Log(problems);
        JSVapor.Console.Dir(orphanElem.GetJSObject());

        //myButton.InvokeFuncProp("click");

        //Window.Alert(myButton.IsFormElement());
    }

}

internal static partial class Test
{
    [SupportedOSPlatform("browser")]
    [JSExport]
    internal static string InvokeTest(string s)
    {
        string resStr = Environment.NewLine + "You called InvokeTest(string s), with s = " + s + "." + Environment.NewLine;

        RunTest();

        return resStr;
    }
}
