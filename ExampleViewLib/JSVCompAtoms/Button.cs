using JSVaporizer;
using JSVNuFlexiArch;
using System.Runtime.Versioning;
using static JSVaporizer.JSVapor;

namespace ExampleViewLib;

public class Button : JSVComponent
{
    public string ButtonId { get; }

    /// <summary>
    /// The initial text on the button.
    /// </summary>
    public string Label { get; set; } = "Click Me";

    /// <summary>
    /// Constructor takes a unique name, calls the base constructor,
    /// and sets up a unique button ID.
    /// </summary>
    public Button(string uniqueName) : base(uniqueName)
    {
        ButtonId = UniqueWithSuffix("ButtonId");
    }

    [SupportedOSPlatform("browser")]
    public void SetLabel(string text)
    {
        // Dynamically update the button's text in the DOM.
        Document.AssertGetElementById(ButtonId).SetProperty("textContent", text);
    }

    [SupportedOSPlatform("browser")]
    public string? GetLabel()
    {
        // Retrieve the current button text from the DOM.
        var propInfo = Document.AssertGetElementById(ButtonId).GetProperty("textContent");
        return propInfo.Value as string;
    }

    /// <summary>
    /// Register a click event on this button. 
    /// </summary>
    /// <param name="funcKey">A unique key for the event handler.</param>
    /// <param name="handler">A delegate that runs when the button is clicked.</param>
    [SupportedOSPlatform("browser")]
    public void OnClick(string funcKey, EventHandlerCalledFromJS handler)
    {
        Document.AssertGetElementById(ButtonId).AddEventListener("click", funcKey, handler);
    }

    /// <summary>
    /// Remove a previously registered click event handler.
    /// </summary>
    [SupportedOSPlatform("browser")]
    public void RemoveOnClick(string funcKey)
    {
        Document.AssertGetElementById(ButtonId).RemoveEventListener("click", funcKey);
    }

    /// <summary>
    /// Return the Handlebars template for this component.
    /// </summary>
    protected override string GetTemplate()
    {
        return @"
                <span id=""{{{UniqueName}}}"">
                    <button id=""{{{ButtonId}}}"">{{{Label}}}</button>
                </span>
            ";
    }
}
