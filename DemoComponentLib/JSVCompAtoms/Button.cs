using JSVaporizer;
using JSVNuFlexiArch;
using System;
using System.Runtime.Versioning;
using static JSVaporizer.JSVapor;

namespace DemoComponentLib;

public class Button : JSVComponent
{
    private IDisposable? _onClickToken;

    public string ButtonId { get; }
    public string Text { get; set; } = "Click Me";

    public Button(string uniqueName) : base(uniqueName)
    {
        ButtonId = UniqueWithSuffix("ButtonId");
    }

    [SupportedOSPlatform("browser")]
    public void SetText(string text)
    {
        Document.AssertGetElementById(ButtonId).SetProperty("textContent", text);
    }

    [SupportedOSPlatform("browser")]
    public string? GetText()
    {
        var propInfo = Document.AssertGetElementById(ButtonId).GetProperty("textContent");
        return propInfo.Value as string;
    }

    [SupportedOSPlatform("browser")]
    public void OnClick(EventListenerCalledFromJS listener)
    {
        _onClickToken = Document.AssertGetElementById(ButtonId).AddEventListener("click", listener);
    }

    [SupportedOSPlatform("browser")]
    public void RemoveOnClick(string funcKey)
    {
        if (_onClickToken != null)
        {
            _onClickToken.Dispose();
        }
    }

    protected override string GetTemplate()
    {
        return @"
                <span id=""{{UniqueName}}"">
                    <button id=""{{ButtonId}}"">{{Text}}</button>
                </span>
            ";
    }
}
