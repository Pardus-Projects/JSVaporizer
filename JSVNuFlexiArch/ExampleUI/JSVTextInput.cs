using Microsoft.AspNetCore.Html;
using System.Runtime.Versioning;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using static JSVaporizer.JSVapor;

namespace JSVNuFlexiArch;

[JsonSerializable(typeof(TextInputDataDto))]
public partial class TextInputSerializerContext : JsonSerializerContext { }
public class TextInputDataDto : CompDataDto
{
    public string? Label { get; set; }
    public string? InputValue { get; set; }
}

[SupportedOSPlatform("browser")]
public class JSVTextInput : JSVComponent
{
    private string? _labelValue;
    private string? _textValue;

    public string Id;
    public string LabelId;
    public JSVTextInput(string uniqueName) : base(uniqueName)
    {
        Id = uniqueName.AppendElementSuffix("InputId");
        LabelId = uniqueName.AppendElementSuffix("LabelId");
    }

    protected void SetLabel(string? val = null)
    {
        _labelValue = val;
        Document.AssertGetElementById(LabelId).SetProperty("innerHTML", val??"");
    }

    protected string? GetLabel()
    {
        return _labelValue;
    }

    protected void SetInputValue(string? val)
    {
        _textValue = val;
        Document.AssertGetElementById(Id).SetFormElemValue(_textValue);
    }

    protected string? GetInputValue()
    {
        return _textValue;
    }

    public override bool UpdateState(CompDataDto tempDto)
    {
        if (tempDto is TextInputDataDto sDto)
        {
            SetLabel(sDto.Label);
            SetInputValue(sDto.InputValue);
            return true;
        }
        throw new ArgumentException("Invalid DTO type for ATextInput.");
    }

    public override CompDataDto GetState()
    {
        var sDto = new TextInputDataDto
        {
            Label = GetLabel(),
            InputValue = GetInputValue()
        };
        return sDto;
    }

    public override JsonTypeInfo GetJsonTypeInfo()
    {
        return TextInputSerializerContext.Default.TextInputDataDto;
    }

    public override void RenderBody(HtmlContentBuilder htmlCB)
    {
        string htmlStr = Environment.NewLine + $"<label id=\"{LabelId}\" for=\"{Id}\"></label>";
        htmlStr += Environment.NewLine + $"<input id=\"{Id}\" type=\"text\"/>";
        htmlCB.AppendHtml(htmlStr);
    }
}


