using JSVNuFlexiArch;
using System.Collections.Generic;
using System.Runtime.Versioning;
using static JSVaporizer.JSVapor;

namespace DemoComponentLib;

public class DropDownList : JSVComponent
{
    public string DropDownId { get; }
    public FormLabel Label { get; set; }

    public List<string> Options { get; set; } = new();

    public DropDownList(string uniqueName) : base(uniqueName)
    {
        DropDownId = UniqueWithSuffix("DropDownId");
        Label = new(UniqueWithSuffix("Label"), DropDownId);
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
        var optionHtml = "";
        foreach (var option in Options)
        {
            optionHtml += $@"<option value=""{option}"">{option}</option>";
        }

        return @"
                <span id=""{{UniqueName}}"">
                    <select id=""" + DropDownId + @""">
                        " + optionHtml + @"
                    </select>
                </span>
            ";
    }
}
