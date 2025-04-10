using System.Text.Json.Serialization.Metadata;
using System.Text.Json.Serialization;

namespace JSVNuFlexiArch;

[JsonSerializable(typeof(DataGridColumn))]
[JsonSerializable(typeof(DataGridRow))]
[JsonSerializable(typeof(DataGridDataDto))]
public partial class DataGridSerializerContext : JsonSerializerContext { }

public class DataGridColumn
{
    public string? Field { get; set; }  // e.g. "Name", "Price"
    public string? Header { get; set; } // e.g. "Product Name"
    public bool IsSortable { get; set; }
}

public class DataGridRow
{
    // Key-value pairs mapping column Field to the cell value.
    public Dictionary<string, object?> Cells { get; set; } = new();
    public bool IsSelected { get; set; }
}

public class DataGridDataDto : CompDataDto
{
    public List<DataGridColumn> Columns { get; set; } = new();
    public List<DataGridRow> Rows { get; set; } = new();
    public string? SortedBy { get; set; } // the field being sorted, if any
    public bool SortAscending { get; set; } = true;
}

public abstract class ADataGrid : JSVComponent
{
    protected ADataGrid(string uniqueName) : base(uniqueName) { }

    protected abstract void SetColumns(List<DataGridColumn> columns);
    protected abstract List<DataGridColumn> GetColumns();

    protected abstract void SetRows(List<DataGridRow> rows);
    protected abstract List<DataGridRow> GetRows();

    protected abstract void SetSortedBy(string? field);
    protected abstract string? GetSortedBy();

    protected abstract void SetSortAscending(bool ascending);
    protected abstract bool GetSortAscending();

    public override bool UpdateState(CompDataDto tempDto)
    {
        if (tempDto is DataGridDataDto dgDto)
        {
            SetColumns(dgDto.Columns);
            SetRows(dgDto.Rows);
            SetSortedBy(dgDto.SortedBy);
            SetSortAscending(dgDto.SortAscending);
            return true;
        }
        throw new ArgumentException("Invalid DTO type for ADataGrid.");
    }

    public override CompDataDto GetState()
    {
        var dto = new DataGridDataDto
        {
            Columns = GetColumns(),
            Rows = GetRows(),
            SortedBy = GetSortedBy(),
            SortAscending = GetSortAscending()
        };
        return dto;
    }

    public override JsonTypeInfo GetJsonTypeInfo()
    {
        return DataGridSerializerContext.Default.DataGridDataDto;
    }
}