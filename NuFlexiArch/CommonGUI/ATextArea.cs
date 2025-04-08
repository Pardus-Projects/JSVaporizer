using System.Text.Json.Serialization.Metadata;
using System.Text.Json.Serialization;

namespace NuFlexiArch;

[JsonSerializable(typeof(TextAreaStateDto))]
public partial class TextAreaSerializerContext : JsonSerializerContext { }

public class TextAreaStateDto : CompStateDto
{
    public string? Label { get; set; }
    public string? TextValue { get; set; }
    public int Rows { get; set; }
    public int Cols { get; set; }
    public int MaxLength { get; set; }
}

public abstract class ATextArea : AComponent
{
    public abstract void SetLabel(string? val);
    public abstract string? GetLabel();
    public abstract void SetTextValue(string? val);
    public abstract string? GetTextValue();
    public abstract void SetRows(int rows);
    public abstract int GetRows();
    public abstract void SetCols(int cols);
    public abstract int GetCols();
    public abstract void SetMaxLength(int maxLength);
    public abstract int GetMaxLength();

    public override bool SetState(CompStateDto tempDto)
    {
        if (tempDto is TextAreaStateDto taDto)
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

    public override CompStateDto GetState()
    {
        var taDto = new TextAreaStateDto
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
        return TextAreaSerializerContext.Default.TextAreaStateDto;
    }
}