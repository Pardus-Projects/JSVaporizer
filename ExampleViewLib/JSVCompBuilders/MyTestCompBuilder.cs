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

        myTestComp.MyCheckBox.Label.Text = "My checkbox";
        
        myTestComp.MyDropDownList.Options = myTestComp.MyList;
        myTestComp.MyDropDownList.Label.Text = "My dropdown list";

        // Radio Button Group & Labels
        foreach (string item in myTestComp.MyList)
        {
            RadioButton rb = new RadioButton(myTestComp.UniqueWithSuffix($"rb_{item}"));
            rb.Name = myTestComp.UniqueWithSuffix("MyRadioGroup");
            myTestComp.MyRadioButtonList.Add(rb);

            FormLabel fl = rb.Label;
            fl.Text = item;
            rb.Name = myTestComp.UniqueWithSuffix("MyRadioGroup");
        }

        myTestComp.MyTextArea.Label.Text = "My textarea";
        myTestComp.MyTextInput.Label.Text = "My text input";

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

    public CheckBox MyCheckBox;
    public DropDownList MyDropDownList;
    public List<RadioButton> MyRadioButtonList { get; set; } = new();
    public TextInput MyTextInput;
    public TextArea MyTextArea;
    public Button MyButton;

    public MyTestComp(string uniqueName) : base(uniqueName)
    {
        MyInt = 42;
        MyCheckBox = new(UniqueWithSuffix("MyCheckBox"));
        MyDropDownList = new(UniqueWithSuffix("MyDropDownList"));
        MyTextInput = new(UniqueWithSuffix("MyTextInput"));
        MyTextArea = new(UniqueWithSuffix("MyTextArea"));
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
                    {{{MyCheckBox}}} {{{MyCheckBox.Label}}}
                </div>

                <div>
                    Radio group:
                    {{#each MyRadioButtonList}}
                        <span>{{{this}}} {{{this.Label}}}</span>
                    {{/each}}
                </div>

                <div>
                    {{{MyDropDownList}}} {{{MyDropDownList.Label}}}
                </div>

                <div>
                    {{{MyTextInput}}}
                    {{{MyTextInput.Label}}}
                </div>

                <div>
                    {{{MyTextArea.Label}}}
                    <br/>
                    {{{MyTextArea}}}
                </div>

                <div>
                    MyButton: {{{MyButton}}}
                </div>
            </div>
        ";
    }
}