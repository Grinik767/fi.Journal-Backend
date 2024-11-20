using System.Globalization;
using Domain.Entities;
using FluentValidation;
using GoogleSheetParser.GoogleSheet;
using GoogleSheetParser.Parser;
using Infrastructure.Repositories;

namespace Application.Services.Users;

public class UsersService(IRepository<User> repository, IValidator<User> validator, IRepository<UserDiff> userDiffRepository, IRepository<Table> tableRepository, ExcelParser excelParser, GoogleSheetManager googleSheetManager)
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
    {
        foreach (var table in group.Tables)
        {
            if (!table.Group.Users.Select(x => x.Id).Contains(user.Id))
                continue;
            
            var path = Path.Combine(Environment.CurrentDirectory, "ExcelTables", $"{table.Name}.xlsx");
            var tempPath = Path.Combine(Environment.CurrentDirectory, "ExcelTables", $"{table.Name}_temp.xlsx");

            if ((DateTime.UtcNow - table.UpdateTime).TotalSeconds < 20 && File.Exists(path))
            {
                var points = await GetStudentsPointFromExistingTable(user, table, path);
                pointsTable.Add(table.Id, points);
            }
            else
            {
                var spreadSheetId = googleSheetManager.GetSpreadSheedId(table.Url);
                await googleSheetManager.DownloadSheetAsXlsx(spreadSheetId, tempPath);
                
                if (File.Exists(path)) 
                    await UpdateUsersDiffs(table, path, tempPath, ct);

                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                    
                File.Move(tempPath, path);

                table.UpdateTime = DateTime.UtcNow;
                await tableRepository.Update(table, ct);
                
                var points = await excelParser.GetStudentsPoints(user.Name, table.StudentColumn, table.HeaderRow, path, table.AdditionalData);
                pointsTable.Add(table.Id, points);
            }
        }
    }

    return pointsTable;
}
    
    private async Task<Dictionary<string, double>> GetStudentsPointFromExistingTable(User user, Table table, string path)
    {
        var points = await excelParser.GetStudentsPoints(user.Name, table.StudentColumn, table.HeaderRow, path, table.AdditionalData);
        return points;
    }

    private async Task UpdateUsersDiffs(Table table, string oldPath, string newPath, CancellationToken ct)
    {
        foreach (var user in table.Group.Users)
        {
            var points = await excelParser.FindDiff(oldPath, newPath, user.Name, table.StudentColumn, table.HeaderRow,
                table.AdditionalData);
            if (points.Count >0)
                await userDiffRepository.Add(new UserDiff(Guid.NewGuid(),
                    table.Id,
                    user.Id,
                    points.ToDictionary(key => key.Key, val => val.Value.ToString(CultureInfo.InvariantCulture))), ct);
        }
    }
}