using Microsoft.AspNetCore.Html;
using NuFlexiArch;
using System.Runtime.Versioning;
using static JSVaporizer.JSVapor;

namespace JSVNuFlexiArch;

[SupportedOSPlatform("browser")]
public class JSVCheckbox : ACheckbox, IJSVComponent
{
    public string Id;
    public string LabelId;
    public JSVCheckbox(string uniqueName)
    {
        Renderer = new JSVCheckboxRenderer();
        Metadata.Add("UnqPrefix", uniqueName);
        Metadata.Add("CompTypeAQN", GetAssemblyQualifiedName());

        Id = uniqueName.AppendElementSuffix("InputId");
        LabelId = uniqueName.AppendElementSuffix("LabelId");
    }

    private string? _labelValue;
    private bool? _isChecked;

    protected override void SetLabel(string? val = null)
    {
        _labelValue = val;
        Document.AssertGetElementById(LabelId).SetProperty("innerHTML", val ?? "");
    }

    protected override string? GetLabel()
    {
        return _labelValue;
    }

    protected override void SetIsChecked(bool isChecked)
    {
        _isChecked = isChecked;
        Document.AssertGetElementById(Id).SetFormElemChecked(_isChecked);
    }

    protected override bool GetIsChecked()
    {
        return _isChecked??false;
    }
}

[SupportedOSPlatform("browser")]
public class JSVCheckboxRenderer : JSVComponentRenderer
{
    protected override void RenderBody(AComponent tmpComp, HtmlContentBuilder htmlCB)
    {
        JSVCheckbox comp = (JSVCheckbox)tmpComp;

        string htmlStr = Environment.NewLine + $"<label id=\"{comp.LabelId}\" for=\"{comp.Id}\"></label>";
        htmlStr += Environment.NewLine + $"<input id=\"{comp.Id}\" type=\"checkbox\"/>";

        htmlCB.AppendHtml(htmlStr);
    }
}