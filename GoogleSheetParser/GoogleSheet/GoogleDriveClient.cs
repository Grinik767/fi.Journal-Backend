using System.CodeDom;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;

namespace GoogleSheetParser.GoogleSheet;

public class GDriveClient
{
    public DriveService DriveService { get; }

    public GDriveClient(string googleAuthJson)
    {
        DriveService = new DriveService(new BaseClientService.Initializer
        {
            HttpClientInitializer = 
                GoogleCredential.FromJson(googleAuthJson).CreateScoped(DriveService.Scope.Drive),
            ApplicationName = "phi-journal"
        });
    }

    public async Task DownloadSheetXlsx(string googleSheetId, string pathToDownload) 
    {
        try
        {
            var request = DriveService.Files.Export(googleSheetId,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            await using var fileStream = new FileStream(pathToDownload, FileMode.Create, FileAccess.Write);
            await request.DownloadAsync(fileStream);
            await fileStream.FlushAsync(); 
            await Task.Delay(100);
        }
        
        catch (Exception exception)
        {
            Console.WriteLine($"При скачивании файла произошла ошибка: {exception.Message}");
        }
    }

    public async Task<string> GetUpdatedFile(string googleSheetId)
    {
        var request = DriveService.Files.Get(googleSheetId);
        request.Fields = "id, name, modifiedTime";

        var result = await request.ExecuteAsync();
        return result.ModifiedTimeRaw;
    }
}