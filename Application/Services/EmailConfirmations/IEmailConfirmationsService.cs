using Domain.Entities;

namespace Application.Services.EmailConfirmations;

public interface IEmailConfirmationsService
{
    Task<EmailConfirmation> CreateEmailConfirmationLink(Guid userId, CancellationToken ct);
    Task SendEmailConfirmationLink(Guid emailConfirmationId, CancellationToken ct);
    Task ConfirmEmail(Guid emailConfirmationId, CancellationToken ct);
}