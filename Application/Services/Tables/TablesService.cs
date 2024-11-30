using Application.Services.UserDiffs;
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
    IUserDiffsService userDiffService,
    ExcelParser excelParser,
    IValidator<Table> validator,
    IRepository<Table> tableRepository) : BaseService<Table>(tablesRepository, validator), ITablesService
{
    private readonly IRepository<Table> _tablesRepository = tablesRepository;

    public async Task<Table> Add(string name, string url, Guid groupId,  int headerRow, string studentColumn, CancellationToken ct, int additionalData=-1)
    {
        var group = await groupsRepository.GetById(groupId, ct);
        return await base.Add(new Table(Guid.NewGuid(), name, url, group.Id, headerRow, studentColumn, additionalData), ct);
    }

    public async Task<Table> Update(Guid id, string? name, CancellationToken ct)
    {
        var table = await _tablesRepository.GetById(id, ct);
        table.Name = name ?? table.Name;
        return await base.Update(table, ct);
    }
    
    public async Task<Dictionary<string, double>> GetStudentPoint(Guid studentId, Guid tableId, CancellationToken ct)
    {
        var user = await usersRepository.GetById(studentId, ct);
        var table = await GetById(tableId, ct);
        if (!table.Group.Users.Select(x => x.Id).ToList().Contains(user.Id))
            return new Dictionary<string, double>();
        var path = Path.Combine(Environment.CurrentDirectory, "ExcelTables", $"{table.Id}.xlsx");
        var spreadSheetId = googleSheetManager.GetSpreadSheedId(table.Url);
        var lastUpdate = await googleSheetManager.GetUpdatedTime(spreadSheetId);
        if (DateTime.Parse(lastUpdate).ToUniversalTime() <= table.UpdateTime && (DateTime.UtcNow - table.UpdateTime).TotalMinutes < 10  && File.Exists(path))
            return await GetStudentsPointFromExistingTable(user, table, path);
        var tempPath = Path.Combine(Environment.CurrentDirectory, "ExcelTables", $"{table.Id}_temp.xlsx");
        
        await googleSheetManager.DownloadSheetAsXlsx(spreadSheetId, tempPath);

        if (File.Exists(path))
        {
            await userDiffService.UpdateUsersDiffs(table, path, tempPath, ct);
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
}