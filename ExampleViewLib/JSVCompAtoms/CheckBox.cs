using JSVNuFlexiArch;
using System.Runtime.Versioning;
using static JSVaporizer.JSVapor;

namespace ExampleViewLib;

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
            <span id=""{{{UniqueName}}}"">
                <input id=""{{{CheckBoxId}}}"" type=""checkbox"" />
                {{LabelText}}
            </span>
        ";
    }
}
