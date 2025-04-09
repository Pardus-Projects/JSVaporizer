using System.Text.Json.Serialization.Metadata;
using System.Text.Json.Serialization;

namespace NuFlexiArch;

[JsonSerializable(typeof(TextAreaDataDto))]
public partial class TextAreaSerializerContext : JsonSerializerContext { }

public class TextAreaDataDto : CompDataDto
{
    public string? Label { get; set; }
    public string? TextValue { get; set; }
    public int Rows { get; set; }
    public int Cols { get; set; }
    public int MaxLength { get; set; }
}

public abstract class ATextArea : AComponent
{
    protected abstract void SetLabel(string? val);
    protected abstract string? GetLabel();
    protected abstract void SetTextValue(string? val);
    protected abstract string? GetTextValue();
    protected abstract void SetRows(int rows);
    protected abstract int GetRows();
    protected abstract void SetCols(int cols);
    protected abstract int GetCols();
    protected abstract void SetMaxLength(int maxLength);
    protected abstract int GetMaxLength();

    public override bool UpdateState(CompDataDto tempDto)
    {
        if (tempDto is TextAreaDataDto taDto)
        {
            SetLabel(taDto.Label);
            SetTextValue(taDto.TextValue);
            SetRows(taDto.Rows);
            SetCols(taDto.Cols);
            SetMaxLength(taDto.MaxLength);
            return true;
        }
        throw new ArgumentException("Invalid DTO type for ATextArea.");
    }

    public override CompDataDto GetState()
    {
        var taDto = new TextAreaDataDto
        {
            Label = GetLabel(),
            TextValue = GetTextValue(),
            Rows = GetRows(),
            MaxLength = GetMaxLength()
        };
        return taDto;
    }

    public override JsonTypeInfo GetJsonTypeInfo()
    {
        return TextAreaSerializerContext.Default.TextAreaDataDto;
    }
}