using Infrastructure.Repositories;
using GoogleSheetParser.GoogleSheet;
using GoogleSheetParser.Parser;
using Domain.Entities;

namespace Application.Services;

public class UsersService(UsersRepository userRepository, GroupsRepository groupsRepository,GoogleSheetManager googleSheetManager, ExcelParser excelParser, TablesRepository tablesRepository)
{
    public async Task<User> Add(string name, string email, string password, CancellationToken ct)
    {
        var user = new User(Guid.NewGuid(), name, email, password);
        await userRepository.Add(user, ct);
        
        return user;
    }

    public async Task Delete(Guid id, CancellationToken ct) => await userRepository.Delete(id, ct);

    public async Task<User?> Update(Guid id, string? email, string? password, CancellationToken ct) =>
        await userRepository.Update(id, email, password, ct);

    public async Task<List<User>> GetAll(CancellationToken ct) => await userRepository.GetAll(ct);

    public async Task<User?> GetById(Guid id, CancellationToken ct) => await userRepository.GetById(id, ct);

    public async Task<Dictionary<Guid, Dictionary<string, double>>> GetUserRecentPoints(Guid id, CancellationToken ct)
    {
        var pointsTable = new Dictionary<Guid, Dictionary<string, double>>();
        var user = await GetById(id, ct);
        foreach (var group in user.Groups)
        foreach (var table in await groupsRepository.GetTables(group.Id, ct))
        {
            var subjectTable = await tablesRepository.GetById(table.Id, ct);
            if (!subjectTable.Group.Users.Select(x => x.Id).ToList().Contains(user.Id))
                continue;
            var spreadSheetId = googleSheetManager.GetSpreadSheedId(subjectTable.Url);
            var path = Path.Combine(Environment.CurrentDirectory, "ExcelTables", $"{subjectTable.Name}.xlsx");
            await googleSheetManager.DownloadSheetAsXlsx(spreadSheetId, path);
            var points = await excelParser.GetStudentsPoints(user.Name, subjectTable.StudentColumn,
                subjectTable.HeaderRow, path, subjectTable.AdditionalData);
            pointsTable.Add(subjectTable.Id, points);
        }
        
        return pointsTable;
    } 
}