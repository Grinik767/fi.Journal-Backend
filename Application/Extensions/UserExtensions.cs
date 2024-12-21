using System.Security.Claims;
using Domain.Entities;

namespace Application.Extensions;

internal static class UserExtensions
{
    public static Claim[] GenerateClaims(this User user) => [new("id", user.Id.ToString())];
}