using Microsoft.AspNetCore.Html;
using System.Reflection.Emit;
using System.Runtime.Versioning;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using static JSVaporizer.JSVapor;

namespace JSVNuFlexiArch;

[JsonSerializable(typeof(TextDisplayDataDto))]
public partial class TextDisplaySerializerContext : JsonSerializerContext { }
public class TextDisplayDataDto : CompDataDto
{
    public string? Text { get; set; }
}

[SupportedOSPlatform("browser")]
public class JSVTextDisplay : JSVComponent
{
    private string? _text;

    public string Id;

    public JSVTextDisplay(string uniqueName) : base(uniqueName)
    {
        Id = uniqueName.AppendElementSuffix("InputId");
    }

    protected void SetText(string? text)
    {
        _text = text;
        Document.AssertGetElementById(Id).SetProperty("innerHTML", _text ?? "");
    }

    protected string? GetText()
    {
        throw new NotImplementedException();
    }

    public override bool UpdateState(CompDataDto tempDto)
    {
        if (tempDto is TextDisplayDataDto tDto)
        {
            SetText(tDto.Text);
            return true;
        }
        throw new ArgumentException("Invalid DTO type for ATextDisplay.");
    }

    public override CompDataDto GetState()
    {
        return new TextDisplayDataDto
        {
            Text = GetText()
        };
    }

    public override JsonTypeInfo GetJsonTypeInfo()
    {
        return TextDisplaySerializerContext.Default.TextDisplayDataDto;
    }

    public override void RenderBody(HtmlContentBuilder htmlCB)
    {
        string htmlStr = Environment.NewLine + $"<span id=\"{Id}\"></span>";
        htmlCB.AppendHtml(htmlStr);
    }
}
