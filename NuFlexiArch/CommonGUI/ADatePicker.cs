using System.Text.Json.Serialization.Metadata;
using System.Text.Json.Serialization;

namespace NuFlexiArch;

[JsonSerializable(typeof(DatePickerDataDto))]
public partial class DatePickerSerializerContext : JsonSerializerContext { }

public class DatePickerDataDto : CompDataDto
{
    public DateTime? SelectedDate { get; set; }
    public DateTime? MinDate { get; set; }
    public DateTime? MaxDate { get; set; }
}

public abstract class ADatePicker : AComponent
{
    protected abstract void SetSelectedDate(DateTime? date);
    protected abstract DateTime? GetSelectedDate();

    protected abstract void SetMinDate(DateTime? date);
    protected abstract DateTime? GetMinDate();

    protected abstract void SetMaxDate(DateTime? date);
    protected abstract DateTime? GetMaxDate();

    public override bool UpdateState(CompDataDto tempDto)
    {
        if (tempDto is DatePickerDataDto dpDto)
        {
            SetSelectedDate(dpDto.SelectedDate);
            SetMinDate(dpDto.MinDate);
            SetMaxDate(dpDto.MaxDate);
            return true;
        }
        throw new ArgumentException("Invalid DTO type for ADatePicker.");
    }

    public override CompDataDto GetState()
    {
        var dto = new DatePickerDataDto
        {
            SelectedDate = GetSelectedDate(),
            MinDate = GetMinDate(),
            MaxDate = GetMaxDate()
        };
        return dto;
    }

    public override JsonTypeInfo GetJsonTypeInfo()
    {
        return DatePickerSerializerContext.Default.DatePickerDataDto;
    }
}