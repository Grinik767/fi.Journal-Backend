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
    ExcelParser excelParser,
    IValidator<Table> validator) : BaseService<Table>(tablesRepository, validator), ITablesService
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
        var spreadSheetId = googleSheetManager.GetSpreadSheedId(table.Url);
        var path = Path.Combine(Environment.CurrentDirectory, "ExcelTables", $"{table.Name}.xlsx");
        await googleSheetManager.DownloadSheetAsXlsx(spreadSheetId, path);
        var points = await excelParser.GetStudentsPoints(user.Name, table.StudentColumn, table.HeaderRow, path, table.AdditionalData);
        return points;
    }
}