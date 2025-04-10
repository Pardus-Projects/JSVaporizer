using System.Text.Json.Serialization.Metadata;
using System.Text.Json.Serialization;

namespace JSVNuFlexiArch;

[JsonSerializable(typeof(DropdownOption))]
[JsonSerializable(typeof(DropdownDataDto))]
public partial class DropdownSerializerContext : JsonSerializerContext { }

public class DropdownOption
{
    public string? Value { get; set; }
    public string? Label { get; set; }
}

public class DropdownDataDto : CompDataDto
{
    public bool AllowMultiple { get; set; }
    public List<DropdownOption> Options { get; set; } = new();
    // For multi-select, store selected values. For single-select, store 1 element or just the first.
    public List<string> SelectedValues { get; set; } = new();
}

public abstract class ADropdown : JSVComponent
{
    protected ADropdown(string uniqueName) : base(uniqueName) { }

    protected abstract void SetAllowMultiple(bool allow);
    protected abstract bool GetAllowMultiple();

    protected abstract void SetOptions(List<DropdownOption> options);
    protected abstract List<DropdownOption> GetOptions();

    protected abstract void SetSelectedValues(List<string> values);
    protected abstract List<string> GetSelectedValues();

    public override bool UpdateState(CompDataDto tempDto)
    {
        if (tempDto is DropdownDataDto ddDto)
        {
            SetAllowMultiple(ddDto.AllowMultiple);
            SetOptions(ddDto.Options);
            SetSelectedValues(ddDto.SelectedValues);
            return true;
        }
        throw new ArgumentException("Invalid DTO type for ADropdown.");
    }

    public override CompDataDto GetState()
    {
        var dto = new DropdownDataDto
        {
            AllowMultiple = GetAllowMultiple(),
            Options = GetOptions(),
            SelectedValues = GetSelectedValues()
        };
        return dto;
    }

    public override JsonTypeInfo GetJsonTypeInfo()
    {
        return DropdownSerializerContext.Default.DropdownDataDto;
    }
}