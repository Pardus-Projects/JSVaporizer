using Microsoft.AspNetCore.Mvc.RazorPages;
using JSVNuFlexiArch;
using NuFlexiArch;

namespace MyExampleApplication.Pages
{
    public class IndexModel : PageModel
    {
        public JSVTextDisplay JSVTextDisplay = new("JSVTextDisplay");
        public string JSVTextDisplay_InstanceJson;

        public JSVTextInput JSVTextInput = new("JSVTextInput");
        public string JSVTextInput_InstanceJson;

        public JSVTextArea JSVTextArea = new("JSVTextArea");
        public string JSVTextArea_InstanceJson;

        public JSVCheckbox JSVCheckbox = new("JSVCheckbox");
        public string JSVCheckbox_InstanceJson;

        public JSVSlider JSVSlider = new("JSVSlider");
        public string JSVSlider_InstanceJson;

        public void OnGet()
        {
            TextDisplayDataDto textDisplayDto = new()
            {
                Text = "JSVTextDisplay TEXT"
            };
            JSVTextDisplay_InstanceJson = JSVTextDisplay.SerializeInstance(textDisplayDto).Serialize();

            TextInputDataDto textInputDataDto = new()
            {
                Label = "JSVTextInput LABEL",
                InputValue = "JSVTextInput VALUE"
            };
            JSVTextInput_InstanceJson = JSVTextInput.SerializeInstance(textInputDataDto).Serialize();

            TextAreaDataDto textAreaDataDto = new()
            {
                Label = "JSVTextArea LABEL",
                TextValue = "JSVTextArea VALUE",
                Rows = 4,
                Cols = 80,
                MaxLength = 90
            };
            JSVTextArea_InstanceJson = JSVTextArea.SerializeInstance(textAreaDataDto).Serialize();

            CheckboxDataDto checkboxDataDto = new()
            {
                Label = "JSVCheckbox LABEL",
                IsChecked = true
            };
            JSVCheckbox_InstanceJson = JSVCheckbox.SerializeInstance(checkboxDataDto).Serialize();

            SliderDataDto sliderDataDto = new()
            {
                Label = "",
                Value = 96,
                MinValue = 7,
                MaxValue = 107,
                Step = 0.5
            };
            JSVSlider_InstanceJson = JSVSlider.SerializeInstance(sliderDataDto).Serialize();
        }
    }
}
