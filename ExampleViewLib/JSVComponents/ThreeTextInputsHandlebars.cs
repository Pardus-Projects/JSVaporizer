using JSVZenView;
using System.Runtime.Versioning;

namespace ExampleViewLib;

[SupportedOSPlatform("browser")]
public class ThreeTextInputsHandlebars : ZenView
{
    public TwoTextInputs Input1and2 { get; }
    public TextInput Input3 { get; }

    public int thing { get; }

    public ThreeTextInputsHandlebars(string uniqueName) : base(uniqueName)
    {
        Input1and2 = new(UniqueWithSuffix("Input1and2"));
        Input3 = new(UniqueWithSuffix("Input3"));
    }

    protected override string GetTemplate()
    {
        string hTemplate = @"
            <div id=""{{{UniqueName}}}"">

                Inputs 1 and 2

                {{{Input1and2}}}

                <hr />

                Input 3

                {{{Input3}}}

            </div>
        ";
        return hTemplate;
    }
}

