using JSVNuFlexiArch;
using System.Runtime.Versioning;
using static JSVaporizer.JSVapor;

namespace DemoComponentLib;

public class TextInput : JSVComponent
{
    public string InputId { get; }
    public FormLabel Label { get; set; }

    public TextInput(string uniqueName) : base(uniqueName)
    {
        InputId = UniqueWithSuffix("InputId");
        Label = new(UniqueWithSuffix("Label"), InputId);
    }

    [SupportedOSPlatform("browser")]

    public void SetInputVal(string? val)
    {
        Document.AssertGetElementById(InputId).SetFormElemValue(val);
    }

    [SupportedOSPlatform("browser")]

    public string? GetInputVal()
    {
        return Document.AssertGetElementById(InputId).GetFormElemValue();
    }

    protected override string GetTemplate()
    {
        string hTemplate = @"
            <span id=""{{UniqueName}}"">
                <input id=""{{InputId}}"" type=""text""/>
            </span>
        ";

        return hTemplate;
    }
}


