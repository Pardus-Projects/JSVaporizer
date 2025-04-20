using JSVaporizer;
using JSVNuFlexiArch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;
using System.Text.Json;
using System.Text.Json.Serialization;
using static JSVaporizer.JSVapor;
using static JSVaporizer.JSVapor.JSVGenericFunctionPool;

namespace DemoComponentLib;

[JsonSerializable(typeof(MyCoolTransformerDto))]
public partial class MyCoolTransformerDtoContext : JsonSerializerContext { }
public class MyCoolTransformerDto : TransformerDto
{
    public string? Name { get; set; }
    public int? Day { get; set; }
    public List<int> OtherDays { get; set; } = new();
    public string? Notes { get; set; }
    public bool? ParallelPetGrooming { get; set; }
}

public class BarberAppointmentTransformer : JSVTransformer
{
    public override MyCoolTransformerDto JsonToDto(string dtoJson)
    {
        MyCoolTransformerDto? dto =  JsonSerializer.Deserialize(dtoJson, MyCoolTransformerDtoContext.Default.MyCoolTransformerDto);
        if (dto == null)
        {
            throw new JSVException("dto is null");
        }
        return dto;
    }

    public override string DtoToJson(TransformerDto dto)
    {
         return JsonSerializer.Serialize(dto, MyCoolTransformerDtoContext.Default.MyCoolTransformerDto);
    }

    [SupportedOSPlatform("browser")]
    public override string DtoToView(string dtoJson, string? userInfoJson = null)
    {
        MyCoolTransformerDto dto = JsonToDto(dtoJson);

        Element txtName = Document.AssertGetElementById("txtName");
        Element dlApptDay = Document.AssertGetElementById("dlApptDay");
        Element txtNotes = Document.AssertGetElementById("txtNotes");
        Element chkParallelPetGrooming = Document.AssertGetElementById("chkParallelPetGrooming");

        // This uses convenience wrappers. Can do manaully.
        txtName.SetFormElemValue(dto.Name);
        dlApptDay.SetFormElemValue(dto.Day);
        txtNotes.SetFormElemValue(dto.Notes);
        chkParallelPetGrooming.SetFormElemChecked(dto.ParallelPetGrooming);

        // Set click listener for btnBookNow
        Element btnBookNow = Document.AssertGetElementById("btnBookNow");
        btnBookNow.AddEventListener("click", BookNowClickListener());

        return "You successfully invoked MyCoolTransformer.Transform().";
    }

    [SupportedOSPlatform("browser")]
    private EventListenerCalledFromJS BookNowClickListener()
    {
        // Register success callback for AjaxPOST().
        RegisterJSVGenericFunction("theSuccessCallback", (object[] args) =>
        {
            Window.Alert("The success callback: " + args[0].ToString());
            return null;
        });
        // Register error callback for AjaxPOST().
        RegisterJSVGenericFunction("theErrorCallback", (object[] args) =>
        {
            Window.Alert("The error callback: " + args[0].ToString());
            return null;
        });

        EventListenerCalledFromJS clickListener = (JSObject elem, string eventType, JSObject evnt) =>
        {
            MyCoolTransformerDto changedDto = ViewToDto();

            // Validate
            bool valid = ValidateDto(changedDto, out string errMessage);
            if (valid)
            {
                string dtoJSON = DtoToJson(changedDto);

                string url = "/BarberAppointmentController/MyRequestHandler";
                JSFunctionPool.CallFunc("AjaxPOST", [url, dtoJSON, "theSuccessCallback", "theErrorCallback"]);

                Window.Alert("Success!");
            }
            else
            {
                Window.Alert(errMessage);
            }

            return (int)JSVEventListenerBehavior.NoDefault_NoPropagate;
        };

        return clickListener;
    }

    [SupportedOSPlatform("browser")]
    public override MyCoolTransformerDto ViewToDto()
    {
        MyCoolTransformerDto dto = new();

        try
        {
            Element txtName = Document.AssertGetElementById("txtName");
            Element dlApptDay = Document.AssertGetElementById("dlApptDay");
            Element txtNotes = Document.AssertGetElementById("txtNotes");
            Element chkParallelPetGrooming = Document.AssertGetElementById("chkParallelPetGrooming");
            Element dlOtherDays = Document.AssertGetElementById("dlOtherDays");

            // This uses convenience wrappers. Can do manaully.
            string? name = txtName.GetFormElemValue();
            string? day = dlApptDay.GetFormElemValue();
            string? notes = txtNotes.GetFormElemValue();
            bool parallelPetGrooming = chkParallelPetGrooming.GetFormElemChecked();

            List<string> selectedDays = dlOtherDays.GetFormElemSelectedList();

            dto.Name = name?.ToString();
            dto.Day = int.Parse(day??"".ToString());
            dto.OtherDays = selectedDays.Select(int.Parse).ToList();
            dto.Notes = notes?.ToString();
            dto.ParallelPetGrooming = parallelPetGrooming;

            return dto;
        }
        catch (Exception ex)
        {
            throw new JSVException(ex.Message);
        }
    }

    public bool ValidateDto(MyCoolTransformerDto dto, out string errMessage)
    {
        bool valid = true;
        errMessage = "";

        if ((dto.Name??"").Trim() == "")
        {
            valid = false;
            errMessage = "Need a name to make an appointment.";
        }
        else if (dto.Day == 0 || dto.Day == null)
        {
            valid = false;
            errMessage = "Need a preferred day to make an appointment.";
        }
        else
        {
            HashSet<int> altDays = new(dto.OtherDays);
            altDays.ExceptWith(new HashSet<int> { (int)dto.Day });
            if (altDays.Count < 2) {
                valid = false;
                errMessage = "Need at least two alternative days different from preferred day.";
            }
        }

        return valid;
    }

}