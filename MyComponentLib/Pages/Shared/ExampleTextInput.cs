using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace JSVComponent;

public class ExampleTextInput : Component
{
    public ExampleTextInput(string unqPrefix) : base(unqPrefix) {  }

    protected override Task RenderBody(IHtmlHelper Html, HtmlContentBuilder htmlCB)
    {
        string htmlStr = Environment.NewLine + $"<input id=\"{CompId}\" type=\"text\"/>";

        htmlCB.AppendHtml(htmlStr);

        return Task.CompletedTask;
    }
}


