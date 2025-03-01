using Quartz;
using Application.Services.Tables;
using Application.Services.UserDiffs;
using Domain.Entities;
using GoogleSheetParser.GoogleSheet;
using Infrastructure.Repositories.Tables;

namespace Application.Jobs;

public class UpdateTablesJob(
    ITablesService tablesService,
    IUserDiffsService userDiffsService,
    IGoogleSheetManager googleSheetManager,
    ITablesRepository tablesRepository) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var ct = new CancellationToken();
        var tables = await tablesService.GetAll(ct);
        foreach (var table in tables)
            await UpdateTable(table, ct);
    }

    private async Task UpdateTable(Table table, CancellationToken ct)
    {
        var path = Path.Combine(Environment.CurrentDirectory, "ExcelTables", $"{table.Id}.xlsx");
        var spreadSheetId = googleSheetManager.GetSpreadSheetId(table.Url);
        if (File.Exists(path) &&
            DateTime.Parse(await googleSheetManager.GetUpdatedTime(spreadSheetId)).ToUniversalTime() <=
            table.UpdateTime && (DateTime.UtcNow - table.UpdateTime).TotalMinutes < 15)
            return;
        
        var tempPath = Path.Combine(Environment.CurrentDirectory, "ExcelTables", $"{table.Id}_temp.xlsx");
        await googleSheetManager.DownloadSheetAsXlsx(spreadSheetId, tempPath);
        
        if (File.Exists(path))
        {
            await userDiffsService.UpdateUsersDiffs(table, path, tempPath, ct);
            File.Delete(path);
        }

        File.Move(tempPath, path);
        table.UpdateTime = DateTime.UtcNow;
        await tablesRepository.Update(table, ct);
    }
}