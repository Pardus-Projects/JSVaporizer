using Microsoft.AspNetCore.Html;
using System.Runtime.Versioning;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using static JSVaporizer.JSVapor;

namespace JSVNuFlexiArch;

[JsonSerializable(typeof(ButtonDataDto))]
public partial class ButtonSerializerContext : JsonSerializerContext { }

public class ButtonDataDto : CompDataDto
{
    public string? Label { get; set; }
    public string? Text { get; set; }
    public bool? IsDisabled { get; set; }
}

[SupportedOSPlatform("browser")]
public class JSVButton : JSVComponent
{
    private string? _labelValue;
    private string? _text;
    private bool? _isDisabled;

    public string LabelId;
    public string Id;

    public JSVButton(string uniqueName) : base(uniqueName)
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

    protected void SetText(string? val = null)
    {
        _text = val;
        Document.AssertGetElementById(Id).SetProperty("innerHTML", val ?? "");
    }

    protected string? GetText()
    {
        return _text;
    }

    protected void SetDisabled(bool? isDisabled)
    {
        _isDisabled = isDisabled;
        Document.AssertGetElementById(Id).SetProperty("disabled", _isDisabled??false);
    }

    protected bool GetDisabled()
    {
        return _isDisabled ?? false;
    }

    public override bool UpdateState(CompDataDto tempDto)
    {
        if (tempDto is ButtonDataDto bDto)
        {
            SetLabel(bDto.Label);
            SetText(bDto.Text);
            SetDisabled(bDto.IsDisabled);
            return true;
        }
        throw new ArgumentException("Invalid DTO type for AButton.");
    }

    public override CompDataDto GetState()
    {
        var dto = new ButtonDataDto
        {
            Label = GetLabel(),
            Text = GetText(),
            IsDisabled = GetDisabled()
        };
        return dto;
    }

    public override JsonTypeInfo GetJsonTypeInfo()
    {
        return ButtonSerializerContext.Default.ButtonDataDto;
    }

    public override void RenderBody(HtmlContentBuilder htmlCB)
    {
        string htmlStr = Environment.NewLine + $"<label id=\"{LabelId}\" for=\"{Id}\"></label>";
        htmlStr += Environment.NewLine + $"<button id=\"{Id}\"></button>";
        htmlCB.AppendHtml(htmlStr);
    }
}

