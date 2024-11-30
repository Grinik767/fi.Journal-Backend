using System.Globalization;
using Domain.Entities;
using GoogleSheetParser.Parser;
using Infrastructure.Repositories.UserDiffs;
using Infrastructure.Repositories.Users;

namespace Application.Services.UserDiffs;

public class UserDiffsService(
    IUsersDiffsRepository userDiffRepository,
    IUsersRepository userRepository,
    ExcelParser excelParser) : IUserDiffsService
{
    public async Task<List<UserDiff>> GetDiffsForUser(Guid userId, CancellationToken ct)
    {
        var user = await userRepository.GetById(userId, ct);
        
        var userDiffs = await userDiffRepository.GetAllByUser(user.Id, ct);
        return userDiffs;
    }

    public async Task UpdateUsersDiffs(Table table, string oldPath, string newPath, CancellationToken ct)
    {
        foreach (var user in table.Group.Users)
        {
            var points = await excelParser.FindDiff(oldPath, newPath, user.Name, table.StudentColumn, table.HeaderRow,
                table.AdditionalData);

            if (points.Count <= 0) continue;

            var usersDiffs = await userDiffRepository.GetAllByTable(table.Id, ct);
            await DeleteUsersDiffs(usersDiffs, ct);

            await userDiffRepository.Add(new UserDiff(Guid.NewGuid(), table.Id, user.Id,
                points.ToDictionary(key => key.Key, val => val.Value.ToString(CultureInfo.InvariantCulture))), ct);
        }
    }

    private async Task DeleteUsersDiffs(List<UserDiff> userDiffs, CancellationToken ct)
    {
        foreach (var userDiff in userDiffs)
            await userDiffRepository.Delete(userDiff.Id, ct);
    }
}