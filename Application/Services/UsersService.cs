using Infrastructure.Repositories;
using Domain.Entities;
using GoogleSheetParser.GoogleSheet;
using GoogleSheetParser.Parser;

namespace Application.Services;

public class UsersService(UsersRepository userRepository, TablesRepository tablesRepository, GoogleSheetManager googleSheetManager, ExcelParser excelParser)
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

    public async Task<Dictionary<string, double>> GetStudentPoint(Guid studentId, Guid tableId, CancellationToken ct)
    {
        var user = await GetById(studentId, ct);
        var table = await tablesRepository.GetById(tableId, ct);
        var spreadSheetId = googleSheetManager.GetSpreadSheedId(table.Url);
        var path = Path.Combine(Environment.CurrentDirectory, "ExcelTables", $"{table.Name}.xlsx");
        Console.WriteLine(user.Name);
        Console.WriteLine(table.StudentColumn);
        Console.WriteLine(table.HeaderRow);
        Console.WriteLine(table.AdditionalData);
        await googleSheetManager.DownloadSheetAsXlsx(spreadSheetId, path);
        var points = await excelParser.GetStudentsPoints(user.Name, table.StudentColumn, table.HeaderRow, path, table.AdditionalData);
        return points;
    }
}