using Microsoft.AspNetCore.Mvc.RazorPages;
using JSVComponent;
using MyTransformerLib;
using System.Text.Json;

namespace MyExampleApplication.Pages
{
    public class IndexModel : PageModel
    {
        public ExampleTextInput MyTextInput = new("myTextInput");
        public ExampleNested MyNested = new("myNested");

        public ExampleComp TheExampleComp = new("theExampleComp");
        public string TheExampleComp_DtoJson = "";
        public string TheExampleComp_CompInfoJson = "";

        public string XformerDtoJSON = "";

        public void OnGet()
        {
            MyExampleCompTransformerDto dto = new()
            {
                HeaderStr = "The Header Info",
                ContentStr = "The Content Info"
            };
            TheExampleComp_DtoJson = JsonSerializer.Serialize(dto);
            TheExampleComp_CompInfoJson = TheExampleComp.SerializeComponentProperties();
        }
    }
}
