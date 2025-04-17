﻿using JSVaporizer;
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
            myTestComp.MyButton.OnClick("MyButton_OnClick", MyButtonClickHandler(myTestComp.MyButton));

            newButton.OnClick("newButton_OnClick", AnotherClickHandler(newButton));
            anotherNewButton.OnClick("anotherNewButton_OnClick", AnotherClickHandler(anotherNewButton));

            myTestComp.MyTabControl.AfterChildrenAttached();
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

    private EventHandlerCalledFromJS AnotherClickHandler(Button btn)
    {
        EventHandlerCalledFromJS clickHandler = (JSObject elem, string eventType, JSObject evnt) =>
        {
            Window.Alert(btn.Text);
            return (int)JSVEventHandlerBehavior.NoDefault_NoPropagate;
        };

        return clickHandler;
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
            <div id="" {{{UniqueName}}} "">

                <div>
                    TabControl: {{{MyTabControl}}}
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

        return template;
    }
}