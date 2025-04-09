using System.Text.Json.Serialization.Metadata;
using System.Text.Json.Serialization;

namespace NuFlexiArch;

[JsonSerializable(typeof(ListBoxItem))]
[JsonSerializable(typeof(ListBoxDataDto))]
public partial class ListBoxSerializerContext : JsonSerializerContext { }

public class ListBoxItem
{
    public string? Value { get; set; }
    public string? Label { get; set; }
}

public class ListBoxDataDto : CompDataDto
{
    public bool AllowMultiple { get; set; }
    public List<ListBoxItem> Items { get; set; } = new();
    public List<string> SelectedValues { get; set; } = new();
}

public abstract class AListBox : AComponent
{
    protected abstract void SetAllowMultiple(bool allow);
    protected abstract bool GetAllowMultiple();

    protected abstract void SetItems(List<ListBoxItem> items);
    protected abstract List<ListBoxItem> GetItems();

    protected abstract void SetSelectedValues(List<string> values);
    protected abstract List<string> GetSelectedValues();

    public override bool UpdateState(CompDataDto tempDto)
    {
        if (tempDto is ListBoxDataDto lbDto)
        {
            SetAllowMultiple(lbDto.AllowMultiple);
            SetItems(lbDto.Items);
            SetSelectedValues(lbDto.SelectedValues);
            return true;
        }
        throw new ArgumentException("Invalid DTO type for AListBox.");
    }

    public override CompDataDto GetState()
    {
        var dto = new ListBoxDataDto
        {
            AllowMultiple = GetAllowMultiple(),
            Items = GetItems(),
            SelectedValues = GetSelectedValues()
        };
        return dto;
    }

    public override JsonTypeInfo GetJsonTypeInfo()
    {
        return ListBoxSerializerContext.Default.ListBoxDataDto;
    }
}