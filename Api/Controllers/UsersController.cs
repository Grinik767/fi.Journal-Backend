using Api.Contracts.User;
using Api.Dtos;
using Api.Extensions;
using Application.Services.EmailConfirmations;
using Application.Services.UserDiffs;
using AutoMapper;
using Application.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController(
    IUsersService service,
    IUserDiffsService userDiffsService,
    IEmailConfirmationsService emailConfirmationsService,
    IMapper mapper) : ControllerBase
{
    [HttpPost("register")]
    [Authorize(Policy = "DenyAuthenticated")]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequest request, CancellationToken ct)
    {
        var user = await service.Register(request.Name, request.Email, request.Password, ct);
        return Ok(mapper.Map<UserDto>(user));
    }

    [HttpPost("login")]
    [Authorize(Policy = "DenyAuthenticated")]
    public async Task<IActionResult> Login([FromBody] LoginUserRequest request, CancellationToken ct)
    {
        var token = await service.Login(request.Email, request.Password, request.Remember, ct);
        return Ok(token);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "SuperAdminOrPersonalDataAccess")]
    public async Task Delete(Guid id, CancellationToken ct) => await service.Delete(id, ct);

    [HttpPatch("{id:guid}")]
    [Authorize(Policy = "SuperAdminOrPersonalDataAccess")]
    public async Task<IActionResult> Update(Guid id, string? email, string? password, CancellationToken ct)
    {
        var user = await service.Update(id, email, password, ct);
        return Ok(mapper.Map<UserDto>(user));
    }

    [HttpGet]
    [Authorize(Policy = "SuperAdmin")]
    public async Task<List<UserDto>> GetAll(CancellationToken ct)
    {
        var users = await service.GetAll(ct);
        return mapper.Map<List<UserDto>>(users);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "SuperAdminOrPersonalDataAccess")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var user = await service.GetById(id, ct);
        return Ok(mapper.Map<UserDto>(user));
    }

    [HttpGet("profile")]
    [Authorize]
    public async Task<IActionResult> GetById(CancellationToken ct) =>
        await GetById(HttpContext.GetUserIdFromHttpContext(), ct);

    [HttpGet("{id:guid}/recentDiffs")]
    [Authorize(Policy = "SuperAdminOrPersonalDataAccess")]
    public async Task<List<UserDiffDto>> GetUsersDiff(Guid id, CancellationToken ct)
    {
        var result = await userDiffsService.GetDiffsForUser(id, ct);
        return mapper.Map<List<UserDiffDto>>(result);
    }

    [HttpPost("{id:guid}/confirmEmail")]
    [Authorize(Policy = "SuperAdminOrPersonalDataAccess")]
    public async Task<IActionResult> SendEmailConfirmationLink(Guid id, CancellationToken ct)
    {
        var emailConfirmation = await emailConfirmationsService.CreateEmailConfirmationLink(id, ct);
        //await emailConfirmationsService.SendEmailConfirmationLink(emailConfirmation.Id, ct);
        return Ok();
    }

    [HttpGet("confirmEmail/{emailConfirmationId:guid}")]
    public async Task<IActionResult> ConfirmEmail(Guid emailConfirmationId, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}