using ExampleViewLib;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MyExampleApplication.Pages
{
    public class IndexModel : PageModel
    {
        public MyTestCompBuilder MyTestCompBuilder;
        public RegistrationFormCompBuilder RegistrationFormCompBuilder;

        public void OnGet()
        {
            MyTestCompBuilder = new MyTestCompBuilder();
            RegistrationFormCompBuilder = new RegistrationFormCompBuilder();
        }

    }
}
