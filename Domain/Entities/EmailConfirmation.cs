using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class EmailConfirmation(Guid id, Guid userId): Entity<Guid>(id)
{
    [Required] public Guid UserId { get; init; } = userId;
    public int Code { get; init; } = GenerateConfirmationCode();

    public User? User { get; }
    
    private static int GenerateConfirmationCode()
    {
        var result = 0;
        for (var i = 0; i < 6; i++)
            result += (int)Math.Pow(10, i) * Random.Shared.Next(0, 10);

        return result;
    }
}