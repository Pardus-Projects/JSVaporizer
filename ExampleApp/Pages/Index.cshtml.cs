using Microsoft.AspNetCore.Mvc.RazorPages;
using ExampleViewLib;

namespace MyExampleApplication.Pages
{
    public class IndexModel : PageModel
    {

        public TextInput TextInput_Server = new("TextInput_Server");

        public TextInput TextInput_Client = new("TextInput_Client");

        public TwoTextInputs TwoTextInputs = new("TwoTextInputs");

        public ThreeTextInputsHandlebars ThreeTextInputsHandlebars = new("ThreeTextInputsHandlebars");

        public void OnGet()
        {

        }
    }
}
