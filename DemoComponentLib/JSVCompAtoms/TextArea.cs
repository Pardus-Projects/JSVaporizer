using JSVNuFlexiArch;
using System.Runtime.Versioning;
using static JSVaporizer.JSVapor;

namespace DemoComponentLib
{
    public class TextArea : JSVComponent
    {
        public string TextAreaId { get; }
        public FormLabel Label { get; set; }
        public int Rows { get; set; } = 8;
        public int Cols { get; set; } = 30;

        public TextArea(string uniqueName) : base(uniqueName)
        {
            TextAreaId = UniqueWithSuffix("TextAreaId");
            Label = new(UniqueWithSuffix("Label"), TextAreaId);
        }
        
        [SupportedOSPlatform("browser")]

        public void SetTextVal(string? val)
        {
            Document.AssertGetElementById(TextAreaId).SetFormElemValue(val);
        }

        [SupportedOSPlatform("browser")]

        public string? GetTextVal()
        {
            return Document.AssertGetElementById(TextAreaId).GetFormElemValue();
        }

        protected override string GetTemplate()
        {
            string hTemplate = @"
                <span id=""{{UniqueName}}"">
                    <textarea 
                        id=""{{TextAreaId}}"" 
                        rows=""{{Rows}}""
                        cols=""{{Cols}}"">
                    </textarea>
                </span>
            ";

            return hTemplate;
        }
    }
}
