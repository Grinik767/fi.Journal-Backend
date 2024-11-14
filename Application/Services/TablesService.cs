using Domain.Entities;
using GoogleSheetParser.GoogleSheet;
using GoogleSheetParser.Parser;
using Infrastructure.Repositories;

namespace Application.Services;

public class TablesService(TablesRepository tablesRepository, GroupsRepository groupsRepository, GoogleSheetManager googleSheetManager, ExcelParser excelParser, UsersRepository usersRepository)
{
    public async Task<Table?> Add(string name, string url, Guid groupId,  int headerRow, string studentColumn, CancellationToken ct, int additionalData=-1)
    {
        var group = await groupsRepository.GetById(groupId, ct);
        if (group is null)
            return null;

        var table = new Table(Guid.NewGuid(), name, url, groupId, headerRow, studentColumn, additionalData);
        await tablesRepository.Add(table, ct);

        return table;
    }

    public async Task Delete(Guid id, CancellationToken ct) =>
        await tablesRepository.Delete(id, ct);

    public async Task<Table?> GetById(Guid id, CancellationToken ct) =>
        await tablesRepository.GetById(id, ct);

    public async Task<List<Table>> GetAll(CancellationToken ct) =>
        await tablesRepository.GetAll(ct);

    public async Task<Table?> Update(Guid id, string? name, CancellationToken ct) =>
        await tablesRepository.Update(id, name, ct);
    
    public async Task<Dictionary<string, double>> GetStudentPoint(Guid studentId, Guid tableId, CancellationToken ct)
    {
        var user = await usersRepository.GetById(studentId, ct);
        var table = await GetById(tableId, ct);
        if (!table.Group.Users.Select(x => x.Id).ToList().Contains(user.Id))
            return new Dictionary<string, double>();
        var spreadSheetId = googleSheetManager.GetSpreadSheedId(table.Url);
        var path = Path.Combine(Environment.CurrentDirectory, "ExcelTables", $"{table.Name}.xlsx");
        await googleSheetManager.DownloadSheetAsXlsx(spreadSheetId, path);
        var points = await excelParser.GetStudentsPoints(user.Name, table.StudentColumn, table.HeaderRow, path, table.AdditionalData);
        return points;
    }
}