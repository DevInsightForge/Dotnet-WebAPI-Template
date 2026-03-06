using DevInsightForge.Application.DtoModels.Authentication;
using DevInsightForge.Application.DtoModels.User;
using DevInsightForge.Application.Features.Authentication.Commands;
using DevInsightForge.Application.Features.Authentication.Queries;
using DevInsightForge.WebAPI.Common.Mappings;
using DevInsightForge.WebAPI.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevInsightForge.WebAPI.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class AuthenticationController(IMediator mediator) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost]
    [ProducesResponseType<ApiResponse>(StatusCodes.Status201Created)]
    public async Task<IActionResult> Register(RegisterRequestDto dto, CancellationToken ct)
    {
        var result = await mediator.Send(new RegisterCommand(dto), ct);
        return result.ToApiResponse();
    }

    [AllowAnonymous]
    [HttpPost]
    [ProducesResponseType<ApiResponse<AuthSessionResponseDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Login(LoginRequestDto dto, CancellationToken ct)
    {
        var result = await mediator.Send(new LoginCommand(dto), ct);
        return result.ToApiResponse();
    }

    [AllowAnonymous]
    [HttpPost]
    [ProducesResponseType<ApiResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> VerifyEmailOtp(VerifyEmailOtpRequestDto dto, CancellationToken ct)
    {
        var result = await mediator.Send(new VerifyEmailOtpCommand(dto), ct);
        return result.ToApiResponse();
    }

    [AllowAnonymous]
    [HttpPost]
    [ProducesResponseType<ApiResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> ResendEmailVerificationOtp(ResendEmailVerificationOtpRequestDto dto, CancellationToken ct)
    {
        var result = await mediator.Send(new ResendEmailVerificationOtpCommand(dto), ct);
        return result.ToApiResponse();
    }

    [HttpGet]
    [ProducesResponseType<ApiResponse<UserResponseModel>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCurrentUser(CancellationToken ct)
    {
        var result = await mediator.Send(new GetCurrentUserQuery(), ct);
        return result.ToApiResponse();
    }
}
