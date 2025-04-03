using Microsoft.AspNetCore.Mvc.RazorPages;
using JSVComponent;
using MyTransformerLib;
using System.Text.Json;

namespace MyExampleApplication.Pages
{
    public class IndexModel : PageModel
    {
        public ExampleComp myExampleComp = new("myExampleComp");
        public ExampleTextInput myTextInput = new("myTextInput");
        public ExampleNested testNested = new("testNested");

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
