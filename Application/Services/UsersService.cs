using Infrastructure.Repositories;
using Domain.Entities;
using GoogleSheetParser.GoogleSheet;
using GoogleSheetParser.Parser;

namespace Application.Services;

public class UsersService(UsersRepository userRepository, TablesRepository tablesRepository)
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
}