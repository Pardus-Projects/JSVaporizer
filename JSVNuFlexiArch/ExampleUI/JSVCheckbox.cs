using Microsoft.AspNetCore.Html;
using JSVNuFlexiArch;
using System.Runtime.Versioning;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using static JSVaporizer.JSVapor;

namespace JSVNuFlexiArch;

[JsonSerializable(typeof(CheckboxDataDto))]
public partial class CheckboxSerializerContext : JsonSerializerContext { }

public class CheckboxDataDto : CompDataDto
{
    public string? Label { get; set; }
    public bool IsChecked { get; set; }
}


[SupportedOSPlatform("browser")]
public class JSVCheckbox : JSVComponent
{
    private string? _labelValue;
    private bool? _isChecked;

    public string Id;
    public string LabelId;
    public JSVCheckbox(string uniqueName) : base(uniqueName)
    {
        Id = uniqueName.AppendElementSuffix("InputId");
        LabelId = uniqueName.AppendElementSuffix("LabelId");
    }

    protected void SetLabel(string? val = null)
    {
        _labelValue = val;
        Document.AssertGetElementById(LabelId).SetProperty("innerHTML", val ?? "");
    }

    protected string? GetLabel()
    {
        return _labelValue;
    }

    protected void SetIsChecked(bool isChecked)
    {
        _isChecked = isChecked;
        Document.AssertGetElementById(Id).SetFormElemChecked(_isChecked);
    }

    protected bool GetIsChecked()
    {
        return _isChecked??false;
    }

    public override bool UpdateState(CompDataDto tempDto)
    {
        if (tempDto is CheckboxDataDto dto)
        {
            SetLabel(dto.Label);
            SetIsChecked(dto.IsChecked);
            return true;
        }
        throw new ArgumentException("Invalid DTO type for ACheckbox.");
    }

    public override CompDataDto GetState()
    {
        return new CheckboxDataDto
        {
            Label = GetLabel(),
            IsChecked = GetIsChecked()
        };
    }

    public override JsonTypeInfo GetJsonTypeInfo()
    {
        return CheckboxSerializerContext.Default.CheckboxDataDto;
    }

    public override void RenderBody(HtmlContentBuilder htmlCB)
    {
        string htmlStr = Environment.NewLine + $"<label id=\"{LabelId}\" for=\"{Id}\"></label>";
        htmlStr += Environment.NewLine + $"<input id=\"{Id}\" type=\"checkbox\"/>";
        htmlCB.AppendHtml(htmlStr);
    }
}