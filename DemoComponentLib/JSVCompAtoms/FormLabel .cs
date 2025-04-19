using JSVNuFlexiArch;
using System.Runtime.Versioning;
using static JSVaporizer.JSVapor;

namespace DemoComponentLib;

public class FormLabel : JSVComponent
{
    public string LabelId { get; }

    public string? Text { get; set; }

    public string? ForInputId { get; set; }

    public FormLabel(string uniqueName, string forInputId) : base(uniqueName)
    {
        LabelId = UniqueWithSuffix("LabelId");
        ForInputId = forInputId;
    }

    protected override string GetTemplate()
    {
        return @"
                <label id=""{{unescaped LabelId}}"" for=""{{unescaped ForInputId}}"">
                    {{Text}}
                </label>
            ";
    }

    [SupportedOSPlatform("browser")]
    public void SetText(string newText)
    {
        Document.AssertGetElementById(LabelId).SetProperty("textContent", newText);
    }

    [SupportedOSPlatform("browser")]
    public string? GetText()
    {
        var labelElem = Document.AssertGetElementById(LabelId);
        var prop = labelElem.GetProperty("textContent");
        return prop.Value as string;
    }
}
