using Google.Apis.Sheets.v4.Data;

namespace GoogleSheetParser.GoogleSheet;

public class GoogleSheetManager
{
    private readonly GDriveClient _driveClient;
    private readonly GoogleSheetCreation _sheetCreation;
    private readonly GoogleSheetReader _sheetReader;
    private readonly GoogleSheetEditor _sheetEditor;

    public GoogleSheetManager(string serviceAccountKeyPath)
    {
        var credentialsPath = Path.Combine(Environment.CurrentDirectory, serviceAccountKeyPath);
        var credentials = File.ReadAllText(credentialsPath);
        _driveClient = new GDriveClient(credentials);
        var sheetClient = new GSheetClient(credentials);
        
        _sheetCreation = new GoogleSheetCreation(sheetClient.SheetsService, _driveClient.DriveService);
        _sheetReader = new GoogleSheetReader(sheetClient.SheetsService);
        _sheetEditor = new GoogleSheetEditor(sheetClient.SheetsService);
    }
    
    public Spreadsheet CreateNewSheet(string sheetTitle)
    {
        var spreadsheetId = _sheetCreation.CreateNewTable(sheetTitle);
        return spreadsheetId;
    }
    
    public GoogleSheetEditor EditSheet() => 
        _sheetEditor;

    public async Task DownloadSheetAsXlsx(string spreadsheetId, string destinationFilePath) => 
        await _driveClient.DownloadSheetXlsx(spreadsheetId, destinationFilePath);

    public Spreadsheet GetSpreadsheet(string spreadsheetId) => 
        _sheetReader.GetSpreadTable(spreadsheetId);

    public static string GetSpreadSheetId(string url) 
        => GSheetClient.GetSpreadsheetId(url);

    public object GetSingleCellValue(string spreadsheetId, string range, string? sheetName = null) => 
        _sheetReader.GetSingleValue(spreadsheetId, range, sheetName).Values.First().First();

    public IList<ValueRange> GetMultipleValues(string spreadsheetId, string[] ranges) => 
        _sheetReader.GetMultipleValues(spreadsheetId, ranges).ValueRanges;

    public IList<IList<object>> GetRowOrColumnValues(string spreadsheetId, string columnOrRow, string? sheetName = null) => 
        _sheetReader.GetColumnOrRowValues(spreadsheetId, columnOrRow, sheetName).Values;

    public IList<IList<object>> GetTableValues(string spreadsheetId, string sheetName) => 
        _sheetReader.GetTableValues(spreadsheetId, sheetName).Values;

    public void AddEditorAccess(string spreadsheetId, string userEmail) => 
        _sheetCreation.AddEditor(spreadsheetId, userEmail);

    public async Task<string> GetUpdatedTime(string spreadSheetId) => 
        await _driveClient.GetUpdatedFile(spreadSheetId);

    public async Task<int> GetSheetGid(string spreadSheetId, string sheetName) =>
        await _sheetReader.GetSheetGid(spreadSheetId, sheetName);
}