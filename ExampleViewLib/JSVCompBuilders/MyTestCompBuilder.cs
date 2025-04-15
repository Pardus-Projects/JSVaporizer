using JSVaporizer;
using JSVNuFlexiArch;
using System.Collections.Generic;
using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;
using static JSVaporizer.JSVapor;

namespace ExampleViewLib;

[SupportedOSPlatform("browser")]
public class MyTestCompBuilder : JSVCompBuilder
{
    public MyTestCompBuilder() : base() { }
    public override JSVComponent Build(string uniqueName)
    {
        MyTestComp myTestComp = new MyTestComp(uniqueName);
        myTestComp.MyString = "Hello, World!";
        myTestComp.MyList = ["A", "B", "C"];
        myTestComp.MyDropDownList.Options = myTestComp.MyList;

        PostAttachToDOMSetup = () =>
        {
            myTestComp.MyButton.OnClick("MyButton_OnClick", MyButtonClickHandler(myTestComp.MyButton));
        };
        return myTestComp;
    }

    private EventHandlerCalledFromJS MyButtonClickHandler(Button btn)
    {
        EventHandlerCalledFromJS clickHandler = (JSObject elem, string eventType, JSObject evnt) =>
        {
            Window.Alert("You clicked me! But you can't do it anymore.");
            btn.RemoveOnClick("MyButton_OnClick");
            return (int)JSVEventHandlerBehavior.NoDefault_NoPropagate;
        };

        return clickHandler;
    }
}

public class MyTestComp : JSVComponent
{
    public int? MyInt { get; set; }
    public string? MyString { get; set; }
    public List<string> MyList { get; set; } = new();
    public DropDownList MyDropDownList;
    public Button MyButton;

    public MyTestComp(string uniqueName) : base(uniqueName)
    {
        MyInt = 42;
        MyDropDownList = new(UniqueWithSuffix("MyDropDownList"));
        MyButton = new(UniqueWithSuffix("MyButton"));
    }
    
    protected override string GetTemplate()
    {
        return @"
            <div id="" {{{UniqueName}}} "">
                This component has UniqueName = ""{{{UniqueName}}}""

                <div>
                    MyInt: {{MyInt}}
                </div>

                <div>
                    MyString: {{MyString}}
                </div>
    
                <div>
                    MyList:
                    <ul>
                        {{#each MyList}}
                        <li>{{this}}</li>
                        {{/each}}
                    </ul>
                </div>

                <div>
                    MyDropDownList: {{{MyDropDownList}}}
                </div>

                <div>
                    MyButton: {{{MyButton}}}
                </div>
            </div>
        ";
    }
}