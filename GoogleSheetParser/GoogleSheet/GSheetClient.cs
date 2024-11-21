using Google.Apis.Sheets.v4;
using System.Text.RegularExpressions;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;

namespace GoogleSheetParser.GoogleSheet;

public class GSheetClient
{
    private static readonly Regex UrlParse = new ("https://docs.google.com/spreadsheets/d/(.+)/edit\\?gid(.+)");
    
    public SheetsService SheetsService { get; }

    public GSheetClient(string googleAuthJson)
    {
        SheetsService = new SheetsService(new BaseClientService.Initializer
        {
            HttpClientInitializer =
                GoogleCredential.FromJson(googleAuthJson).CreateScoped(SheetsService.Scope.Spreadsheets),
            ApplicationName = "phi-journal"
        });
    }
    
    public static string GetSpreadsheetId(string spreadsheetIdOrUrl)
    {
        var match = UrlParse.Match(spreadsheetIdOrUrl);
        return match.Success ? match.Groups[1].Value : spreadsheetIdOrUrl;
    }
}