using DevInsightForge.Application.Authentication.Commands.AuthenticateUser;
using DevInsightForge.Application.Authentication.Commands.RefreshAccessToken;
using DevInsightForge.Application.Authentication.Commands.RegisterUser;
using DevInsightForge.Application.Authentication.Queries.GetTokenUser;
using DevInsightForge.Application.Common.ViewModels.Authentication;
using DevInsightForge.Application.Common.ViewModels.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevInsightForge.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthenticationController(IMediator mediator) : ControllerBase
{
    [HttpGet(nameof(GetTokenUser))]
    public async Task<UserResponseModel> GetTokenUser(CancellationToken cancellationToken)
    {
        return await mediator.Send(new GetTokenUserQuery(), cancellationToken);
    }

    [AllowAnonymous]
    [HttpPost(nameof(RegisterUser))]
    public async Task<TokenResponseModel> RegisterUser(RegisterUserCommand command, CancellationToken cancellationToken)
    {
        return await mediator.Send(command, cancellationToken);
    }

    [AllowAnonymous]
    [HttpPost(nameof(AuthenticateUser))]
    public async Task<TokenResponseModel> AuthenticateUser(AuthenticateUserCommand command, CancellationToken cancellationToken)
    {
        return await mediator.Send(command, cancellationToken);
    }

    [AllowAnonymous]
    [HttpPost(nameof(RefreshAccessToken))]
    public async Task<TokenResponseModel> RefreshAccessToken(RefreshAccessTokenCommand command, CancellationToken cancellationToken)
    {
        return await mediator.Send(command, cancellationToken);
    }
}
