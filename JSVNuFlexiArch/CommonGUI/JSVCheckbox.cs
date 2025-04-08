using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using NuFlexiArch;
using System.Runtime.Versioning;
using static JSVaporizer.JSVapor;

namespace JSVNuFlexiArch;

[SupportedOSPlatform("browser")]
public class JSVCheckbox : ACheckbox, IJSVComponent
{
    private IComponentRenderer _renderer;
    public string Id;
    public string LabelId;
    public JSVCheckbox(string unqPrefix)
    {
        _renderer = new JSVCheckboxRenderer();
        Metadata.Add("UnqPrefix", unqPrefix);
        Metadata.Add("CompTypeAQN", GetAssemblyQualifiedName());

        Id = JSVComponentHelpers.AppendElementSuffix(unqPrefix, "InputId");
        LabelId = JSVComponentHelpers.AppendElementSuffix(unqPrefix, "LabelId");
    }
    public IComponentRenderer GetRenderer() => _renderer;

    private string? _labelValue;
    private bool? _isChecked;

    public override void SetLabel(string? val = null)
    {
        _labelValue = val;
        Document.AssertGetElementById(LabelId).SetProperty("innerHTML", val ?? "");
    }

    public override string? GetLabel()
    {
        return _labelValue;
    }

    public override void SetIsChecked(bool isChecked)
    {
        _isChecked = isChecked;
        Document.AssertGetElementById(Id).SetFormElemChecked(_isChecked);
    }

    public override bool GetIsChecked()
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