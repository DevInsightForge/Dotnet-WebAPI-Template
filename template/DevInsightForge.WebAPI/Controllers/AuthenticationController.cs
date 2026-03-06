using DevInsightForge.Application.Features.Authentication.Commands;
using DevInsightForge.Application.Features.Authentication.Queries;
using DevInsightForge.Application.DtoModels.Authentication;
using DevInsightForge.Application.DtoModels.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevInsightForge.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthenticationController(IMediator mediator) : ControllerBase
{
    [HttpGet(nameof(GetTokenUser))]
    public async Task<UserResponseModel> GetTokenUser(CancellationToken ct)
    {
        return await mediator.Send(new GetTokenUserQuery(), ct);
    }

    [AllowAnonymous]
    [HttpPost(nameof(RegisterUser))]
    public async Task<TokenResponseModel> RegisterUser(RegisterUserDto dto, CancellationToken ct)
    {
        return await mediator.Send(new RegisterUserCommand(dto), ct);
    }

    [AllowAnonymous]
    [HttpPost(nameof(AuthenticateUser))]
    public async Task<TokenResponseModel> AuthenticateUser(AuthenticateUserDto dto, CancellationToken ct)
    {
        return await mediator.Send(new AuthenticateUserCommand(dto), ct);
    }
}


