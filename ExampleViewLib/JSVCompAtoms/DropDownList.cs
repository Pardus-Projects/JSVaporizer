using JSVNuFlexiArch;
using System;
using System.Collections.Generic;
using System.Runtime.Versioning;
using static JSVaporizer.JSVapor;

namespace ExampleViewLib;

public class DropDownList : JSVComponent
{
    public string DropDownId { get; }

    // This holds the list of options that we’ll render
    public List<string> Options { get; set; } = new();

    public DropDownList(string uniqueName) : base(uniqueName)
    {
        DropDownId = UniqueWithSuffix("DropDownId");
    }

    [SupportedOSPlatform("browser")]
    public void SetSelectedValue(string? value)
    {
        var elem = Document.AssertGetElementById(DropDownId);
        elem.SetFormElemValue(value);
    }

    [SupportedOSPlatform("browser")]
    public string? GetSelectedValue()
    {
        return Document.AssertGetElementById(DropDownId).GetFormElemValue();
    }

    protected override string GetTemplate()
    {
        // We'll dynamically build <option> tags. Note that
        // you can also do fancy stuff with inline Handlebars here.
        // For now, let's just do naive string concatenation.

        var optionHtml = "";
        foreach (var option in Options)
        {
            optionHtml += $@"<option value=""{option}"">{option}</option>";
        }

        // Return a <select> wrapped by a <span>
        return @"
                <span id=""{{{UniqueName}}}"">
                    <select id=""" + DropDownId + @""">
                        " + optionHtml + @"
                    </select>
                </span>
            ";
    }
}
