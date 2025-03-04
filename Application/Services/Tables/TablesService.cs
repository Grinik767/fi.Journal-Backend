using Domain.Entities;
using FluentValidation;
using GoogleSheetParser.GoogleSheet;
using GoogleSheetParser.Parser;
using Infrastructure.Repositories.Groups;
using Infrastructure.Repositories.Tables;
using Infrastructure.Repositories.Users;

namespace Application.Services.Tables;

public class TablesService(
    ITablesRepository tablesRepository,
    IGroupsRepository groupsRepository,
    IUsersRepository usersRepository,
    IGoogleSheetManager googleSheetManager,
    IExcelParser excelParser,
    IValidator<Table> validator) : BaseService<Table>(tablesRepository, validator), ITablesService
{
    public async Task<Table> Add(string name, string url, Guid groupId, int headerRow, string studentColumn,
        CancellationToken ct, int additionalData = -1, string listToSearch = "", string regulationsUrl = "")
    {
        var group = await groupsRepository.GetById(groupId, ct);
        return await base.Add(
            new Table(Guid.NewGuid(), name, url, group.Id, headerRow, studentColumn, additionalData, listToSearch,
                regulationsUrl), ct);
    }

    public async Task<Table> Update(Guid id, string? name, string? regulationsUrl, CancellationToken ct)
    {
        var table = await tablesRepository.GetById(id, ct);
        table.Name = name ?? table.Name;
        table.RegulationsUrl = regulationsUrl ?? table.RegulationsUrl;
        return await base.Update(table, ct);
    }

    public async Task<Table> GetTableWithCustomUrl(Guid tableId, Guid studentId, CancellationToken ct)
    {
        var user = await usersRepository.GetById(studentId, ct);
        var table = await GetById(tableId, ct);
        var path = Path.Combine(Environment.CurrentDirectory, "ExcelTables", $"{table.Id}.xlsx");
        var spreadSheetId = googleSheetManager.GetSpreadSheetId(table.Url);
        
        if (!File.Exists(path))
            await googleSheetManager.DownloadSheetAsXlsx(spreadSheetId, path);
        
        var studentRow = await excelParser.GetStudentRow(table.StudentColumn, user.Name, path, table.ListToSearch);
        var listGid = await googleSheetManager.GetSheetGid(spreadSheetId, studentRow.sheetName);
        if (listGid == -1)
            return table;
        var url =
            $"https://docs.google.com/spreadsheets/d/{spreadSheetId}/edit#gid={listGid}&range={studentRow.studentRow}:{studentRow.studentRow}";
        return new Table(table.Id, table.Name, url, table.Group!.Id, table.HeaderRow, table.StudentColumn,
            table.AdditionalData, table.ListToSearch, table.RegulationsUrl);
    }

    public async Task<Dictionary<string, double>> GetStudentPoint(Guid studentId, Guid tableId, CancellationToken ct)
    {
        var user = await usersRepository.GetById(studentId, ct);
        var table = await GetById(tableId, ct);
        if (!table.Group.Users.Select(x => x.Id).ToList().Contains(user.Id))
            return new Dictionary<string, double>();
        var path = Path.Combine(Environment.CurrentDirectory, "ExcelTables", $"{table.Id}.xlsx");
        var spreadSheetId = googleSheetManager.GetSpreadSheetId(table.Url);
        if (File.Exists(path))
            return await GetStudentsPointFromExistingTable(user, table, path);
        
        await googleSheetManager.DownloadSheetAsXlsx(spreadSheetId, path);
        
        table.UpdateTime = DateTime.UtcNow;
        await tablesRepository.Update(table, ct);

        return await GetStudentsPointFromExistingTable(user, table, path);
    }

    private async Task<Dictionary<string, double>> GetStudentsPointFromExistingTable(User user, Table table,
        string path)
    {
        var points = await excelParser.GetStudentsPoints(user.Name, table.StudentColumn, table.HeaderRow, path,
            table.AdditionalData, true, table.ListToSearch);
        return ChangeOrderToStartFromSum(points);
    }

    private static Dictionary<string, double> ChangeOrderToStartFromSum(Dictionary<string, double> dict)
    {
        var result = new Dictionary<string, double>();
        var priorityKeys = new[] { "брс", "итого", "итог", "сумма", "общий" };
        foreach (var key in priorityKeys)
        {
            foreach (var keyFromDict in dict.Keys.Where(keyFromDict => key == keyFromDict.ToLower().Split(' ', ':')[0]))
            {
                result[keyFromDict] = dict[keyFromDict];
                dict.Remove(keyFromDict);
            }
        }

        foreach (var key in dict.Keys)
            result[key] = dict[key];

        return result;
    }
}