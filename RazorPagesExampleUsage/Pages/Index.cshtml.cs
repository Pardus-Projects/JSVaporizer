using Microsoft.AspNetCore.Mvc.RazorPages;
using MyViewLib;

namespace MyExampleApplication.Pages
{
    public class IndexModel : PageModel
    {
        public JSVTextInput MyTextInput = new("myTextInput", new TextInputRenderer());
        public string MyTextInput_MetadataJson = "";
        public string MyTextInput_StateDtoJson = "";

        public void OnGet()
        {
            MyTextInput_MetadataJson = MyTextInput.SerializeMetadata();
            TextInputStateDto stateDto = new()
            {
                LabelValue = "MY LABEL VALUE",
                InputValue = "MY INPUT VALUE"
            };
            MyTextInput_StateDtoJson = MyTextInput.SerializeState(stateDto);

            //IJSVComponent.InitializeFromJson(MyTextInput_MetadataJson, MyTextInput_StateDtoJson);
        }
    }
}
