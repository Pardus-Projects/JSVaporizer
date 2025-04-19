using JSVNuFlexiArch;
using System.Runtime.Versioning;
using static JSVaporizer.JSVapor;

namespace DemoComponentLib;

public class CheckBox : JSVComponent
{
    public string CheckBoxId { get; }
    public FormLabel Label { get; set; }

    public CheckBox(string uniqueName) : base(uniqueName)
    {
        CheckBoxId = UniqueWithSuffix("CheckBoxId");
        Label = new(UniqueWithSuffix("Label"), CheckBoxId);
    }

    [SupportedOSPlatform("browser")]

    public void SetChecked(bool isChecked)
    {
        Document.AssertGetElementById(CheckBoxId).SetFormElemChecked(isChecked);
    }

    [SupportedOSPlatform("browser")]

    public bool GetChecked()
    {
        return Document.AssertGetElementById(CheckBoxId).GetFormElemChecked();
    }

    protected override string GetTemplate()
    {
        return @"
            <span id=""{{unescaped UniqueName}}"">
                <input id=""{{unescaped CheckBoxId}}"" type=""checkbox"" />
            </span>
        ";
    }
}
