using Microsoft.AspNetCore.Mvc.RazorPages;
using JSVComponent;
using MyTransformerLib;
using System.Text.Json;

namespace RazorPagesJSVaporizer.Controllers
{
    public class IndexModel : PageModel
    {
        //public JSVComponent myBaseComp = new("myBaseComp");
        //public JSVTextInput myTextInput = new("myTextInput");
        public ExampleNested testNested = new("testNested");

        public ExampleComp TheExampleComp = new("theExampleComp");
        public string TheExampleComp_DtoJson = "";
        public string TheExampleComp_CompInfoJson = "";

        public string XformerDtoJSON = "";

        public void OnGet()
        {
            MyExampleCompTransformerDto dto = new MyExampleCompTransformerDto
            {
                HeaderStr = "The Header Info",
                ContentStr = "The Content Info"
            };
            TheExampleComp_DtoJson = JsonSerializer.Serialize(dto);
            TheExampleComp_CompInfoJson = TheExampleComp.SerializeComponentProperties();

            MyCoolTransformerDto xformerDto = new();
            xformerDto.Name = "Bob McBob";
            xformerDto.Day = 3;
            xformerDto.Notes = "I need a shave and a haircut.";
            xformerDto.ParallelPetGrooming = false;
            XformerDtoJSON = JsonSerializer.Serialize(xformerDto);
        }
    }
}
