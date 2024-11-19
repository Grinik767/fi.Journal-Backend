using Domain.Entities;
using FluentValidation;
using GoogleSheetParser.GoogleSheet;
using GoogleSheetParser.Parser;
using Infrastructure.Repositories;

namespace Application.Services.Users;

public class UsersService(IRepository<User> repository, IValidator<User> validator, ExcelParser excelParser, GoogleSheetManager googleSheetManager)
    : BaseService<User>(repository, validator), IUsersService
{
    private readonly IRepository<User> _repository = repository;

    public async Task<User> Add(string name, string email, string password, CancellationToken ct) =>
        await base.Add(new User(Guid.NewGuid(), name, email, password), ct);

    public async Task<User> Update(Guid id, string? email, string? password, CancellationToken ct)
    {
        var user = await _repository.GetById(id, ct);
        user.Email = email ?? user.Email;
        user.PasswordHash = password ?? user.PasswordHash;

        return await base.Update(user, ct);
    }

    public async Task<List<Group>> GetGroups(Guid id, CancellationToken ct)
    {
        var user = await _repository.GetById(id, ct);
        return user.Groups.ToList();
    }

    public async Task<List<Group>> GetGroupsAsAdmin(Guid id, CancellationToken ct)
    {
        var user = await _repository.GetById(id, ct);
        return user.GroupsAsAdmin.ToList();
    }
    
    public async Task<Dictionary<Guid, Dictionary<string, double>>> GetUserRecentPoints(Guid id, CancellationToken ct)
    {
        var pointsTable = new Dictionary<Guid, Dictionary<string, double>>();
        var user = await GetById(id, ct);
        foreach (var group in user.Groups)
        foreach (var table in group.Tables)
        {
            if (!table.Group.Users.Select(x => x.Id).ToList().Contains(user.Id))
                continue;
            var spreadSheetId = googleSheetManager.GetSpreadSheedId(table.Url);
            var path = Path.Combine(Environment.CurrentDirectory, "ExcelTables", $"{table.Name}.xlsx");
            await googleSheetManager.DownloadSheetAsXlsx(spreadSheetId, path);
            var points = await excelParser.GetStudentsPoints(user.Name, table.StudentColumn, table.HeaderRow, path, table.AdditionalData);
            pointsTable.Add(table.Id, points);
        }

        return pointsTable;
    } 
}