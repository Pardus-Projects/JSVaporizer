using Microsoft.AspNetCore.Mvc.RazorPages;
using ExampleViewLib;
using System.Text.Json;

namespace RazorPagesExampleUsage.Pages.BarberAppointmentModel;

public class BarberAppointmentModel : PageModel
{
    public string XformerDtoJSON = "";
    public string XformerAQN = "";

    public void OnGet()
    {
        MyCoolTransformerDto xformerDto = new()
        {
            Name = "Bob McBob",
            Day = 3,
            Notes = "I need a shave and a haircut.",
            ParallelPetGrooming = false
        };

        XformerDtoJSON = JsonSerializer.Serialize(xformerDto);
        XformerAQN = new BarberAppointmentTransformer().AssemblyQualifiedName;
    }
}
