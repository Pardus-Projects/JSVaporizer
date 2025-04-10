using Microsoft.AspNetCore.Html;
using System.Runtime.Versioning;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using static JSVaporizer.JSVapor;

namespace JSVNuFlexiArch;

[JsonSerializable(typeof(TextAreaDataDto))]
public partial class TextAreaSerializerContext : JsonSerializerContext { }

public class TextAreaDataDto : CompDataDto
{
    public string? Label { get; set; }
    public string? TextValue { get; set; }
    public int Rows { get; set; }
    public int Cols { get; set; }
    public int MaxLength { get; set; }
}

[SupportedOSPlatform("browser")]
public class JSVTextArea : JSVComponent
{
    private string? _labelVal;
    private string? _textVal;
    private int _rows;
    private int _cols;
    private int _maxLength;

    public string Id { get; set; }
    public string LabelId { get; set; }

    public JSVTextArea(string uniqueName) : base(uniqueName)
    {
        Id = uniqueName.AppendElementSuffix("InputId");
        LabelId = uniqueName.AppendElementSuffix("LabelId");
    }

    protected void SetLabel(string? val)
    {
        _labelVal = val;
        Document.AssertGetElementById(LabelId).SetProperty("innerHTML", _labelVal ?? "");
    }

    protected string? GetLabel()
    {
        return _labelVal;
    }

    protected void SetTextValue(string? val)
    {
        _textVal = val;
        Document.AssertGetElementById(Id).SetFormElemValue(_textVal);
    }

    protected string? GetTextValue()
    {
        return _textVal;
    }

    protected void SetRows(int rows)
    {
        _rows = rows;
       Document.AssertGetElementById(Id).SetProperty("rows", _rows);
    }

    protected int GetRows()
    {
        return _rows;
    }

    protected void SetCols(int cols)
    {
        _cols = cols;
        Document.AssertGetElementById(Id).SetProperty("cols", _cols);
    }

    protected int GetCols()
    {
        return _cols;
    }

    protected void SetMaxLength(int maxLength)
    {
        _maxLength = maxLength;
        Document.AssertGetElementById(Id).SetProperty("maxLength", _maxLength);
    }

    protected int GetMaxLength()
    {
        return _maxLength;
    }

    public override bool UpdateState(CompDataDto tempDto)
    {
        if (tempDto is TextAreaDataDto taDto)
        {
            SetLabel(taDto.Label);
            SetTextValue(taDto.TextValue);
            SetRows(taDto.Rows);
            SetCols(taDto.Cols);
            SetMaxLength(taDto.MaxLength);
            return true;
        }
        throw new ArgumentException("Invalid DTO type for ATextArea.");
    }

    public override CompDataDto GetState()
    {
        var taDto = new TextAreaDataDto
        {
            Label = GetLabel(),
            TextValue = GetTextValue(),
            Rows = GetRows(),
            MaxLength = GetMaxLength()
        };
        return taDto;
    }

    public override JsonTypeInfo GetJsonTypeInfo()
    {
        return TextAreaSerializerContext.Default.TextAreaDataDto;
    }

    public override void RenderBody(HtmlContentBuilder htmlCB)
    {
        string htmlStr = Environment.NewLine + $"<label id=\"{LabelId}\" for=\"{Id}\"></label>";
        htmlStr += Environment.NewLine + $"<textarea id=\"{Id}\"></textarea>";
        htmlCB.AppendHtml(htmlStr);
    }
}
