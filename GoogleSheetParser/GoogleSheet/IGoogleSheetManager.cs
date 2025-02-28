using Google.Apis.Sheets.v4.Data;

namespace GoogleSheetParser.GoogleSheet;

public interface IGoogleSheetManager
{
    public Spreadsheet CreateNewSheet(string sheetTitle);
    public GoogleSheetEditor EditSheet();
    public Task DownloadSheetAsXlsx(string spreadsheetId, string destinationFilePath);
    public Spreadsheet GetSpreadSheet(string spreadsheetId);
    public object GetSingleCellValue(string spreadsheetId, string range, string? sheetName = null);
    public IList<ValueRange> GetMultipleValues(string spreadsheetId, string[] ranges);

    public IList<IList<object>>
        GetRowOrColumnValues(string spreadsheetId, string columnOrRow, string? sheetName = null);

    public IList<IList<object>> GetTableValues(string spreadsheetId, string sheetName);
    public void AddEditorAccess(string spreadsheetId, string userEmail);
    public Task<string> GetUpdatedTime(string spreadSheetId);

    public string GetSpreadSheetId(string url);

    public Task<int> GetSheetGid(string spreadSheetId, string sheetName);
}