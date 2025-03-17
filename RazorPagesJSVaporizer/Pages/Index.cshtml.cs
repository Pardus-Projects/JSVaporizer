using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using JSVaporizer;

namespace RazorPagesJSVaporizer.Controllers
{
    public class IndexModel : PageModel
    {
        public string XformerDtoJSON = "";

        public void OnGet()
        {
            MyCoolTransformerDto xformerDto = new();

            xformerDto.Name = "Bob McBob";
            xformerDto.Day = 3;
            xformerDto.Notes = "I need a shave and a haircut.";
            xformerDto.ParallelPetGrooming = false;

            XformerDtoJSON = JsonSerializer.Serialize(xformerDto);

        }
    }
}
