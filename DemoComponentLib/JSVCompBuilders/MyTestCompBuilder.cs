using JSVaporizer;
using JSVNuFlexiArch;
using System.Collections.Generic;
using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;
using static JSVaporizer.JSVapor;

namespace DemoComponentLib;

[SupportedOSPlatform("browser")]
public class MyTestCompBuilder : JSVCompBuilder
{
    public MyTestCompBuilder() : base() { }
    public override JSVComponent Build(string uniqueName)
    {
        MyTestComp myTestComp = new MyTestComp(uniqueName);

        // Add a couple of tabs to TabControl
        Button newButton = new(myTestComp.UniqueWithSuffix("newButton"));
        newButton.Text = "Brand new button!";
        Button anotherNewButton = new(myTestComp.UniqueWithSuffix("anotherNewButton"));
        anotherNewButton.Text = "Another brand new button!";
        myTestComp.MyTabControl.SetItems(new List<ContainerItemProto>
        {
            new TabItemProto("My Tab #1", newButton),
            new TabItemProto("My Tab #2", anotherNewButton)
        });

        myTestComp.MyList = ["A", "B", "C"];
        myTestComp.MyCheckBox.Label.Text = "My checkbox";
        myTestComp.MyDropDownList.Options = myTestComp.MyList;
        myTestComp.MyDropDownList.Label.Text = "My dropdown list";

        // Radio Button Group & Labels
        foreach (string item in myTestComp.MyList)
        {
            RadioButton rb = new RadioButton(myTestComp.UniqueWithSuffix($"rb_{item}"));
            rb.Name = myTestComp.UniqueWithSuffix("MyRadioGroup");
            rb.Label.Text = item;
            myTestComp.MyRadioButtonList.Add(rb);
        }

        myTestComp.MyTextArea.Label.Text = "My textarea";
        myTestComp.MyTextInput.Label.Text = "My text input";

        PostAttachToDOMSetup = () =>
        {
            myTestComp.MyButton.OnClick(MyButtonClickListener(myTestComp.MyButton));

            newButton.OnClick(AnotherClickListener(newButton));
            anotherNewButton.OnClick(AnotherClickListener(anotherNewButton));

            myTestComp.MyTabControl.AfterChildrenAttached();
        };

        return myTestComp;
    }

    private EventListenerCalledFromJS MyButtonClickListener(Button btn)
    {
        EventListenerCalledFromJS clickListener = (JSObject elem, string eventType, JSObject evnt) =>
        {
            Window.Alert("You clicked me! But you can't do it anymore.");

#if (DEBUG)
            Window.Alert("Global listener count BEFORE: " + EventListenerDebugInfo.MapKeyCount());
#endif

            btn.RemoveOnClick("MyButton_OnClick");

#if (DEBUG)
            Window.Alert("Global listener count AFTER: " + EventListenerDebugInfo.MapKeyCount());
#endif

            return (int)JSVEventListenerBehavior.NoDefault_NoPropagate;
        };

        return clickListener;
    }

    private EventListenerCalledFromJS AnotherClickListener(Button btn)
    {
        EventListenerCalledFromJS clickListener = (JSObject elem, string eventType, JSObject evnt) =>
        {
            Window.Alert(btn.Text);
            return (int)JSVEventListenerBehavior.NoDefault_NoPropagate;
        };

        return clickListener;
    }
}

public class MyTestComp : JSVComponent
{
    public List<string> MyList { get; set; } = new();

    public TabControl MyTabControl;

    public CheckBox MyCheckBox;
    public DropDownList MyDropDownList;
    public List<RadioButton> MyRadioButtonList { get; set; } = new();
    public TextInput MyTextInput;
    public TextArea MyTextArea;
    public Button MyButton;

    public MyTestComp(string uniqueName) : base(uniqueName)
    {
        MyTabControl = new(UniqueWithSuffix("MyTabControl"));
        MyCheckBox = new(UniqueWithSuffix("MyCheckBox"));
        MyDropDownList = new(UniqueWithSuffix("MyDropDownList"));
        MyTextInput = new(UniqueWithSuffix("MyTextInput"));
        MyTextArea = new(UniqueWithSuffix("MyTextArea"));
        MyButton = new(UniqueWithSuffix("MyButton"));
    }
    
    protected override string GetTemplate()
    {
        string template = @"
            <div id="" {{UniqueName}} "">

                <div>
                    TabControl: {{unescaped MyTabControl}}
                </div>

                <div>
                    {{unescaped MyCheckBox}} {{unescaped MyCheckBox.Label}}
                </div>

                <div>
                    Radio group:
                    {{#each MyRadioButtonList}}
                        <span>{{unescaped this}} {{unescaped this.Label}}</span>
                    {{/each}}
                </div>

                <div>
                    {{unescaped MyDropDownList}} {{unescaped MyDropDownList.Label}}
                </div>

                <div>
                    {{unescaped MyTextInput}}
                    {{unescaped MyTextInput.Label}}
                </div>

                <div>
                    {{unescaped MyTextArea.Label}}
                    <br/>
                    {{unescaped MyTextArea}}
                </div>

                <div>
                    MyButton: {{unescaped MyButton}}
                </div>
            </div>
        ";

        return template;
    }
}