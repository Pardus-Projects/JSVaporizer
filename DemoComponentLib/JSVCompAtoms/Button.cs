﻿using JSVaporizer;
using JSVNuFlexiArch;
using System.Runtime.Versioning;
using static JSVaporizer.JSVapor;

namespace DemoComponentLib;

public class Button : JSVComponent
{
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
    public void OnClick(string funcKey, EventHandlerCalledFromJS handler)
    {
        Document.AssertGetElementById(ButtonId).AddEventListener("click", funcKey, handler);
    }

    [SupportedOSPlatform("browser")]
    public void RemoveOnClick(string funcKey)
    {
        Document.AssertGetElementById(ButtonId).RemoveEventListener("click", funcKey);
    }

    protected override string GetTemplate()
    {
        return @"
                <span id=""{{{UniqueName}}}"">
                    <button id=""{{{ButtonId}}}"">{{{Text}}}</button>
                </span>
            ";
    }
}
