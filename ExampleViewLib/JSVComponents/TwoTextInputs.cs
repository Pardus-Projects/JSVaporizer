using JSVZenView;
using System.Runtime.Versioning;

namespace ExampleViewLib;

[SupportedOSPlatform("browser")]
public class TwoTextInputs : ZenView
{
    public TextInput Input1 { get; }
    public TextInput Input2 { get; }

    public TwoTextInputs(string uniqueName) : base(uniqueName)
    {
        Input1 = new(UniqueWithSuffix("Input1"));
        Input2 = new(UniqueWithSuffix("Input2"));
    }

    protected override string GetTemplate()
    {
        string hTemplate = @"
            <div id=""{{{UniqueName}}}"">

                Input 1

                {{{Input1}}}

                <hr />

                Input 2

                {{{Input2}}}

            </div>
        ";
        return hTemplate;
    }
}

