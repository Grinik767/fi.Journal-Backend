using Google.Apis.Sheets.v4;
using System.Text.RegularExpressions;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;

namespace GoogleSheetParser.GoogleSheet;

public partial class GSheetClient(string googleAuthJson)
{
    private static readonly Regex UrlParse = MyRegex();
    
    public SheetsService SheetsService { get; } = new(new BaseClientService.Initializer
    {
        HttpClientInitializer =
            GoogleCredential.FromJson(googleAuthJson).CreateScoped(SheetsService.Scope.Spreadsheets),
        ApplicationName = "phi-journal"
    });

    public static string GetSpreadsheetId(string spreadsheetIdOrUrl)
    {
        var match = UrlParse.Match(spreadsheetIdOrUrl);
        return match.Success ? match.Groups[1].Value : spreadsheetIdOrUrl;
    }

    [GeneratedRegex("https://docs.google.com/spreadsheets/d/(.+)/edit\\?gid(.+)")]
    private static partial Regex MyRegex();
}