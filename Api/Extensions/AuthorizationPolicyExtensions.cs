using Api.Handlers;
using Microsoft.AspNetCore.Authorization;

namespace Api.Extensions;

public static class AuthorizationPolicyExtensions
{
    /// <summary>
    /// Объединение нескольких политик
    /// </summary>
    public static void RequireCombinedPolicies(this AuthorizationPolicyBuilder builder, params string[] policies) =>
        builder.Requirements.Add(new CombinedAuthorizationRequirement(policies));
}