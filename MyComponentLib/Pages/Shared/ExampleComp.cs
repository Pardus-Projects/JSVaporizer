using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace JSVComponent;

public class ExampleComp : Component
{
    public string HeaderId { get; set; }
    public string ContentId { get; set; }

    public ExampleComp(string unqPrefix) : base(unqPrefix)
    {
        HeaderId = MakeSubComponentUnqPrefix("HeaderId");
        ContentId = MakeSubComponentUnqPrefix("ContentId");
    }

    protected override async Task RenderBody(IHtmlHelper Html, HtmlContentBuilder htmlCB)
    {
        IHtmlContent htmlContent = await Html.PartialAsync("ExampleComp", this);
        htmlCB.AppendHtml(htmlContent);
    }
}
