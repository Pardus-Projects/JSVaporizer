using DemoComponentLib;
using JSVNuFlexiArch;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static JSVaporizer.JSVapor;

namespace DemoApp.Pages
{
    public class IndexModel : PageModel
    {
        public MyTestCompBuilder MyTestCompBuilder;
        public RegistrationFormCompBuilder RegistrationFormCompBuilder;

        public void OnGet()
        {
            MyTestCompBuilder = new MyTestCompBuilder();
            RegistrationFormCompBuilder = new RegistrationFormCompBuilder();

            //MyTestComp comp = (MyTestComp)MyTestCompBuilder.Build("MyTestCompUniqueName");
            //comp.Render();
        }

    }
}
