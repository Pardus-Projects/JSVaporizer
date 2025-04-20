using JSVaporizer;
using JSVNuFlexiArch;
using System.Collections.Generic;
using System.Runtime.Versioning;
using static JSVaporizer.JSVapor;

namespace DemoComponentLib;

[SupportedOSPlatform("browser")]
public class RegistrationFormCompBuilder : JSVCompBuilder
{
    public override JSVComponent Build(string uniqueName)
    {
        var comp = new RegistrationFormComp(uniqueName);

        comp.GenderRadioButtons = CreateGenderRadioGroup(comp.UniqueWithSuffix("Gender"), new List<string> { "Female", "Male", "Other" });
        comp.TermsCheckbox.Label.Text = "I accept the terms of service";
        comp.NameInput.Label.Text = "Full Name:";
        comp.EmailInput.Label.Text = "Email:";
        comp.RegisterButton.Text = "Register";

        // PostAttachToDOM => set up event listeners
        PostAttachToDOMSetup = () =>
        {
            // Attach a click event to the Register button
            comp.RegisterButton.OnClick((elem, evtType, evnt) =>
            {
                // Validate fields
                string nameVal = comp.NameInput.GetInputVal() ?? "";
                string emailVal = comp.EmailInput.GetInputVal() ?? "";
                bool accepted = comp.TermsCheckbox.GetChecked();
                string? gender = null;

                foreach (var rb in comp.GenderRadioButtons)
                {
                    if (rb.IsChecked()) { gender = rb.Value; }
                }

                if (string.IsNullOrWhiteSpace(nameVal) || string.IsNullOrWhiteSpace(emailVal) || string.IsNullOrWhiteSpace(gender) || !accepted)
                {
                    Window.Alert("Please complete all fields and accept the terms!");
                }
                else
                {
                    Window.Alert($"Registration successful!\nName: {nameVal}\nEmail: {emailVal}\nGender: {gender}");
                }

                return (int)JSVEventListenerBehavior.NoDefault_NoPropagate;
            });
        };

        return comp;
    }

    private List<RadioButton> CreateGenderRadioGroup(string groupPrefix, List<string> options)
    {
        // We want them all in one group, so create one group name
        string radioGroupName = groupPrefix + "_RG";

        var radioList = new List<RadioButton>();
        foreach (string option in options)
        {
            // Each radio gets a unique id, but they share the same 'Name'
            var rb = new RadioButton(groupPrefix + "_" + option)
            {
                Name = radioGroupName,
                Value = option
            };
            rb.Label.Text = option;
            radioList.Add(rb);
        }
        return radioList;
    }
}

public class RegistrationFormComp : JSVComponent
{
    public TextInput NameInput { get; }
    public TextInput EmailInput { get; }
    public List<RadioButton> GenderRadioButtons { get; set; } = new();
    public CheckBox TermsCheckbox { get; }
    public Button RegisterButton { get; }

    public RegistrationFormComp(string uniqueName) : base(uniqueName)
    {
        NameInput = new TextInput(UniqueWithSuffix("NameInput"));
        EmailInput = new TextInput(UniqueWithSuffix("EmailInput"));
        TermsCheckbox = new CheckBox(UniqueWithSuffix("TermsCheckbox"));
        RegisterButton = new Button(UniqueWithSuffix("RegisterButton"));
    }

    protected override string GetTemplate()
    {
        // Render each input plus its label. For radio buttons, we iterate the list.
        return @"
                <div id=""{{UniqueName}}"">
                    <h2>Registration Form</h2>

                    <div>
                        {{unescaped NameInput}} {{unescaped NameInput.Label}}
                    </div>

                    <div>
                        {{unescaped EmailInput}} {{unescaped EmailInput.Label}}
                    </div>

                    <div>
                        <h4>Gender</h4>
                        {{#each GenderRadioButtons}}
                            <span>{{unescaped this}} {{unescaped this.Label}}</span>
                        {{/each}}
                    </div>

                    <div>
                        {{unescaped TermsCheckbox}} {{unescaped TermsCheckbox.Label}}
                    </div>

                    <div>
                        {{unescaped RegisterButton}}
                    </div>
                </div>
            ";
    }
}
