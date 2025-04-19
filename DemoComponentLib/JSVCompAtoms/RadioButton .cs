using JSVNuFlexiArch;
using System.Runtime.Versioning;
using static JSVaporizer.JSVapor;

namespace DemoComponentLib;

public class RadioButton : JSVComponent
{
    public string RadioId { get; }
    public string? Name { get; set; }
    public string? Value { get; set; }
    public FormLabel Label { get; set; }

    public RadioButton(string uniqueName) : base(uniqueName)
    {
        RadioId = UniqueWithSuffix("RadioId");
        Label = new(UniqueWithSuffix("Label"), RadioId);
    }

    [SupportedOSPlatform("browser")]
    public void SetChecked(bool isChecked)
    {
        Document.AssertGetElementById(RadioId).SetFormElemChecked(isChecked);
    }

    [SupportedOSPlatform("browser")]
    public bool IsChecked()
    {
        return Document.AssertGetElementById(RadioId).GetFormElemChecked();
    }

    protected override string GetTemplate()
    {
        return @"
                <span id=""{{UniqueName}}"">
                    <input
                        id=""{{RadioId}}""
                        type=""radio""
                        name=""{{Name}}""
                        value=""{{Value}}""
                    />
                    {{LabelText}}
                </span>
            ";
    }
}
