using System.Collections.Generic;
using System.Runtime.Versioning;

namespace JSVaporizer;

[SupportedOSPlatform("browser")]
public static partial class JSVapor
{
    public static readonly HashSet<string> WhitelistedFormElements = new HashSet<string>
    {
        "INPUT",
        "TEXTAREA",
        "SELECT",
        "DATALIST",
        "OPTION",
        "OPTGROUP",
        "OUTPUT",
        "BUTTON"
    };

    public static string? PropToString(this Element element, string propName)
    {
        ElementPropInfo? propInfo = element.GetProperty(propName);
        if (propInfo == null)
        {
            return null;
        }

        return propInfo.Value?.ToString();
    }

    public static string GetTagName(this Element element)
    {
        return element.PropToString("tagName") ?? "";
    }

    public static bool IsFormElement(this Element element)
    {
        return WhitelistedFormElements.Contains(element.GetTagName());
    }

    public static string SetFormElemValue(this Element element, object? objVal)
    {
        if (!element.IsFormElement())
        {
            throw new JSVException($"element with id={element.Id} has tagName={element.GetTagName()} and is not whitelisted form element.");
        }
        else
        {
            element.AssertProperty("value");
        }

        string val = objVal?.ToString() ?? "";
        element.SetProperty("value", val);

        return val;
    }

    public static string? GetFormElemValue(this Element element)
    {
        if (!element.IsFormElement())
        {
            throw new JSVException($"element with id={element.Id} has tagName={element.GetTagName()} and is not whitelisted form element.");
        }
        else
        {
            element.AssertProperty("value");
        }

        return element.PropToString("value");
    }

    public static bool SetFormElemChecked(this Element element, bool? nIsChecked)
    {
        bool isChecked = nIsChecked ?? false;
        element.SetProperty("checked", isChecked);

        return isChecked;
    }

    public static bool GetFormElemChecked(this Element element)
    {
        if (!element.IsFormElement())
        {
            throw new JSVException($"element with id={element.Id} has tagName={element.GetTagName()} and is not a whitelisted form element.");
        }
        else
        {
            element.AssertProperty("checked");
        }

        string strChecked = "" + element.PropToString("checked");

        return strChecked.ToLower() == "true";
    }

    public static List<string> GetFormElemSelectedList(this Element element)
    {
        if (!element.IsFormElement())
        {
            throw new JSVException($"element with id={element.Id} has tagName={element.GetTagName()} and is not a whitelisted form element.");
        }
        else
        {
            if (element.GetTagName() != "SELECT")
            {
                throw new JSVException($"element with id={element.Id} has tagName={element.GetTagName()} and is not handled.");
            }

            element.AssertProperty("multiple");
            element.AssertProperty("options");
        }

        return element.GetMultiSelectOptionValues();
    }
    

}
