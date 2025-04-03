using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace JSVComponent;

public class ExampleTextInput : Component
{
    public string InputId { get; set; }
    public ExampleTextInput(string unqPrefix) : base(unqPrefix)
    {
        InputId = AppendElementSuffix("InputId");
    }

    protected override Task RenderBody(IHtmlHelper Html, HtmlContentBuilder htmlCB)
    {
        string htmlStr = Environment.NewLine + $"<input id=\"{InputId}\" type=\"text\"/>";

        htmlCB.AppendHtml(htmlStr);

        return Task.CompletedTask;
    }
}


