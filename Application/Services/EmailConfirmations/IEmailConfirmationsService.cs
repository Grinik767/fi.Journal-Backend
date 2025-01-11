using Domain.Entities;

namespace Application.Services.EmailConfirmations;

public interface IEmailConfirmationsService
{
    Task<EmailConfirmation> CreateEmailConfirmationLink(Guid userId, CancellationToken ct);
    Task<EmailConfirmation?> CreateChangePasswordLink(string email, CancellationToken ct);
    Task SendEmailConfirmationLink(Guid id, CancellationToken ct);
    Task SendChangePasswordLink(Guid id, CancellationToken ct);
    Task ConfirmEmail(Guid emailConfirmationId, CancellationToken ct);
    Task<string> ChangePassword(Guid emailConfirmationId, CancellationToken ct);
}