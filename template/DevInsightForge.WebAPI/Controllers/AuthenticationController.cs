using DevInsightForge.Application.Features.Authentication.Commands;
using DevInsightForge.Application.Features.Authentication.Queries;
using DevInsightForge.Application.DtoModels.Authentication;
using DevInsightForge.Application.DtoModels.User;
using DevInsightForge.WebAPI.Common.Mappings;
using DevInsightForge.WebAPI.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevInsightForge.WebAPI.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class AuthenticationController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<ApiResponse<UserResponseModel>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTokenUser(CancellationToken ct)
    {
        var result = await mediator.Send(new GetTokenUserQuery(), ct);
        return result.ToApiResponse();
    }

    [AllowAnonymous]
    [HttpPost]
    [ProducesResponseType<ApiResponse<TokenResponseModel>>(StatusCodes.Status201Created)]
    public async Task<IActionResult> RegisterUser(RegisterUserDto dto, CancellationToken ct)
    {
        var result = await mediator.Send(new RegisterUserCommand(dto), ct);
        return result.ToApiResponse();
    }

    [AllowAnonymous]
    [HttpPost]
    [ProducesResponseType<ApiResponse<TokenResponseModel>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> AuthenticateUser(AuthenticateUserDto dto, CancellationToken ct)
    {
        var result = await mediator.Send(new AuthenticateUserCommand(dto), ct);
        return result.ToApiResponse();
    }
}
