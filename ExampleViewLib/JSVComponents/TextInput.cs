using JSVZenView;
using System;
using System.Runtime.Versioning;
using static JSVaporizer.JSVapor;

namespace ExampleViewLib;

[SupportedOSPlatform("browser")]
public class TextInput : ZenView
{
    public string InputId { get; }

    private string? _inputValue;

    public TextInput(string uniqueName) : base(uniqueName)
    {
        InputId = UniqueWithSuffix("InputId");
    }

    public string GetInputId()
    {
        return InputId;
    }

    public void SetInputVal(string? val)
    {
        _inputValue = val;
        Document.AssertGetElementById(InputId).SetFormElemValue(_inputValue);
    }

    public string? GetInputVal()
    {
        return _inputValue;
    }

    protected override string GetTemplate()
    {
        string hTemplate = Environment.NewLine + @"
            <span id=""{{{UniqueName}}}"">
                <input id=""{{{InputId}}}"" type=""text""/>
            </span>
        ";

        return hTemplate;
    }
}


