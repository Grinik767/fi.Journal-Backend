using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;

namespace GoogleSheetParser.GoogleSheet;

public class GoogleSheetCreation(SheetsService sheetsService, DriveService driveService)
{
    private SheetsService SheetsService { get; } = sheetsService;
    private DriveService DriveService { get; } = driveService;

    public Spreadsheet CreateNewTable(string dockName)
    {
        if (string.IsNullOrEmpty(dockName))
            throw new ArgumentNullException(nameof(dockName), "Передано пустое имя таблицы при создании");
        
        var documentCreationRequest = SheetsService.Spreadsheets.Create(new Spreadsheet
        {
            Sheets = new List<Sheet> { new() { Properties = new SheetProperties { Title = dockName } } },
            Properties = new SpreadsheetProperties { Title = dockName }
        });

        var spreadsheet = documentCreationRequest.Execute();
        
        GrantReadAccess(spreadsheet.SpreadsheetId);

        return spreadsheet;
    }

    private void GrantReadAccess(string spreadsheetId)
    {
        var permission = new Permission
        {
            Type = "anyone",
            Role = "reader"
        };

        DriveService.Permissions.Create(permission, spreadsheetId).Execute();
    }

    public void AddEditor(string spreadsheetId, string emailAddress)
    {
        if (string.IsNullOrEmpty(emailAddress))
            throw new ArgumentNullException(nameof(emailAddress), "Не передана почта");

        var permission = new Permission
        {
            Type = "user",
            Role = "writer",
            EmailAddress = emailAddress
        };

        DriveService.Permissions.Create(permission, spreadsheetId).Execute();
    }
}