using System.Globalization;
using Domain.Entities;
using FluentValidation;
using Infrastructure.Repositories;
using GoogleSheetParser.GoogleSheet;
using GoogleSheetParser.Parser;
using Infrastructure.Repositories.Users;

namespace Application.Services.Tables;

public class TablesService(
    IRepository<Table> tablesRepository,
    IRepository<Group> groupsRepository,
    IUsersRepository usersRepository,
    GoogleSheetManager googleSheetManager,
    IRepository<UserDiff> userDiffRepository,
    ExcelParser excelParser,
    IValidator<Table> validator,
    IRepository<Table> tableRepository) : BaseService<Table>(tablesRepository, validator), ITablesService
{
    private readonly IRepository<Table> _tablesRepository = tablesRepository;

    public async Task<Table> Add(string name, string url, Guid groupId,  int headerRow, string studentColumn, CancellationToken ct, int additionalData=-1)
    {
        var group = await groupsRepository.GetById(groupId, ct);
        return await base.Add(new Table(Guid.NewGuid(), name, url, groupId, headerRow, studentColumn, additionalData), ct);
    }

    public async Task<Table> Update(Guid id, string? name, CancellationToken ct)
    {
        var table = await _tablesRepository.GetById(id, ct);
        var currentPathToTable = Path.Combine(Environment.CurrentDirectory, "ExcelTables", $"{table.Name}.xlsx");
        if (File.Exists(currentPathToTable))
        {
            var newPathToTable = Path.Combine(Environment.CurrentDirectory, "ExcelTables", $"{name}.xlsx");
            File.Move(currentPathToTable, newPathToTable);
        }
        
        table.Name = name ?? table.Name;

        return await base.Update(table, ct);
    }

    public async Task<Table> UpdateTime(Guid tableId, CancellationToken ct)
    {
        var currentTime = DateTime.UtcNow;
        var table = await _tablesRepository.GetById(tableId, ct);
        table.UpdateTime = currentTime;
        return await _tablesRepository.Update(table, ct);
    }
    
    public async Task<Dictionary<string, double>> GetStudentPoint(Guid studentId, Guid tableId, CancellationToken ct)
    {
        var user = await usersRepository.GetById(studentId, ct);
        var table = await GetById(tableId, ct);
        if (!table.Group.Users.Select(x => x.Id).ToList().Contains(user.Id))
            return new Dictionary<string, double>();
        var path = Path.Combine(Environment.CurrentDirectory, "ExcelTables", $"{table.Name}.xlsx");
        if (!((DateTime.UtcNow - table.UpdateTime).TotalMinutes >= 20) && File.Exists(path))
            return await GetStudentsPointFromExistingTable(user, table, path);
        var tempPath = Path.Combine(Environment.CurrentDirectory, "ExcelTables", $"{table.Name}_temp.xlsx");
        var spreadSheetId = googleSheetManager.GetSpreadSheedId(table.Url);
        await googleSheetManager.DownloadSheetAsXlsx(spreadSheetId, tempPath);

        if (File.Exists(path))
        {
            await UpdateUsersDiffs(table, path, tempPath, ct);
            File.Delete(path);
        }

        File.Move(tempPath, path);
        table.UpdateTime = DateTime.UtcNow;
        await tableRepository.Update(table, ct);

        return await GetStudentsPointFromExistingTable(user, table, path);
    }
    
    private async Task<Dictionary<string, double>> GetStudentsPointFromExistingTable(User user, Table table,
        string path)
    {
        var points = await excelParser.GetStudentsPoints(user.Name, table.StudentColumn, table.HeaderRow, path,
            table.AdditionalData);
        return points;
    }

    private async Task UpdateUsersDiffs(Table table, string oldPath, string newPath, CancellationToken ct)
    {
        foreach (var user in table.Group.Users)
        {
            var points = await excelParser.FindDiff(oldPath, newPath, user.Name, table.StudentColumn, table.HeaderRow,
                table.AdditionalData);
            if (points.Count > 0)
                await userDiffRepository.Add(new UserDiff(Guid.NewGuid(), table.Id, user.Id,
                    points.ToDictionary(key => key.Key, val => val.Value.ToString(CultureInfo.InvariantCulture))), ct);
        }
    }
}