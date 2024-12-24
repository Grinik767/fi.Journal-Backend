using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;

namespace GoogleSheetParser.GoogleSheet;

public class GoogleSheetReader(SheetsService sheetsService)
{
    public Spreadsheet GetSpreadTable(string googleSpreadSheetIdentifier)
    {
        CheckForNull(googleSpreadSheetIdentifier);
        return sheetsService.Spreadsheets.Get(googleSpreadSheetIdentifier).Execute();
    }

    public async Task<int> GetSheetGid(string spreadSheetId, string sheetName)
    {
        var request = sheetsService.Spreadsheets.Get(spreadSheetId);
        var sheet = await request.ExecuteAsync();
        var lists = sheet.Sheets;
        foreach (var list in lists)
            if (list.Properties.Title == sheetName)
                return list.Properties.SheetId!.Value;
        return -1;
    }

    public ValueRange GetSingleValue(string googleSpreadSheetIdentifier, string valueRange, string? sheetName = null)
    {
        CheckForNull(googleSpreadSheetIdentifier);
        CheckForNull(valueRange);
        var columnIndex = sheetName is null ? valueRange : $"{sheetName}!{valueRange}";

        return MakeRequestsForValues(googleSpreadSheetIdentifier, columnIndex);
    }

    public BatchGetValuesResponse GetMultipleValues(string googleSpreadSheetIdentifier, string[] ranges)
    {
        CheckForNull(googleSpreadSheetIdentifier);
        if (ranges is null || ranges.Length == 0)
            throw new ArgumentNullException("Не задан диапазон значений");
        
        var getValueRequest = sheetsService.Spreadsheets.Values.BatchGet(googleSpreadSheetIdentifier);
        getValueRequest.Ranges = ranges;
        return getValueRequest.Execute();
    }

    public ValueRange GetColumnOrRowValues(string googleSpreadSheetIdentifier, string column, string? sheetName = null)
    {
        CheckForNull(googleSpreadSheetIdentifier);
        CheckForNull(column);
        var columnIndex = sheetName is null ? $"!{column}:{column}" : $"{sheetName}!{column}:{column}";

        return MakeRequestsForValues(googleSpreadSheetIdentifier, columnIndex);
    }

    public ValueRange GetTableValues(string googleSpreadSheetIdentifier, string sheetName)
    {
        CheckForNull(googleSpreadSheetIdentifier);
        CheckForNull(sheetName);
        
        return MakeRequestsForValues(googleSpreadSheetIdentifier, sheetName);
    }

    private ValueRange MakeRequestsForValues(string googleSpreadSheetIdentifier, string valueRange)
    {
        var getValuesRequest = sheetsService.Spreadsheets.Values.Get(googleSpreadSheetIdentifier, valueRange);
        return getValuesRequest.Execute();
    }

    private static void CheckForNull(string str)
    {
        if (string.IsNullOrEmpty(str))
            throw new ArgumentNullException("Нет значения");
    }
}