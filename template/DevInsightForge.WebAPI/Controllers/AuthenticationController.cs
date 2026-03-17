using DevInsightForge.Application.Contracts.Authentication;
using DevInsightForge.Application.Contracts.User;
using DevInsightForge.Application.Features.Authentication.Commands;
using DevInsightForge.Application.Features.Authentication.Queries;
using DevInsightForge.WebAPI.Contracts.Attributes;
using DevInsightForge.WebAPI.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevInsightForge.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthenticationController(IMediator mediator) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("authenticate")]
    [SuccessResponse<AuthSessionResponseDto>]
    public async Task<ActionResult<AuthSessionResponseDto>> Login(LoginRequestDto request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new LoginCommand(request), cancellationToken);
        return result.ToOkActionResult();
    }

    [AllowAnonymous]
    [HttpPost("register")]
    [NoContentResponse]
    public async Task<IActionResult> Register(RegisterRequestDto request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new RegisterCommand(request), cancellationToken);
        return result.ToNoContentActionResult();
    }

    [AllowAnonymous]
    [HttpPost("verify-email-otp")]
    [NoContentResponse]
    public async Task<IActionResult> VerifyEmailOtp(VerifyEmailOtpRequestDto request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new VerifyEmailOtpCommand(request), cancellationToken);
        return result.ToNoContentActionResult();
    }

    [AllowAnonymous]
    [HttpPost("resend-email-otp")]
    [NoContentResponse]
    public async Task<IActionResult> ResendEmailVerificationOtp(ResendEmailVerificationOtpRequestDto request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new ResendEmailVerificationOtpCommand(request), cancellationToken);
        return result.ToNoContentActionResult();
    }

    [HttpGet("authenticated-user")]
    [SuccessResponse<UserResponseDto>]
    public async Task<ActionResult<UserResponseDto>> GetCurrentUser(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetCurrentUserQuery(), cancellationToken);
        return result.ToOkActionResult();
    }
}


