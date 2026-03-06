using DevInsightForge.Application.DtoModels.Authentication;
using DevInsightForge.Application.Abstructions;
using DevInsightForge.Application.Abstructions.DataAccess;
using DevInsightForge.Domain.Entities;
using DevInsightForge.Application.Results;
using System.Security.Claims;

namespace DevInsightForge.Application.Features.Authentication.Commands;

public sealed record RegisterUserCommand(RegisterUserDto Dto) : IRequest<RegisterUserCommand, Task<Result<TokenResponseModel>>>;

internal sealed class RegisterUserCommandHandler(
    IEncryptionService encryptionService,
    IEmailService emailService,
    IUnitOfWork unitOfWork,
    ITokenService tokenServices,
    IValidator<RegisterUserDto> registerUserDtoValidator) : IRequestHandler<RegisterUserCommand, Task<Result<TokenResponseModel>>>
{
    public async Task<Result<TokenResponseModel>> Handle(RegisterUserCommand request, CancellationToken ct)
    {
        var validationResult = await registerUserDtoValidator.ValidateAsync(request.Dto, ct);
        if (!validationResult.IsValid)
        {
            return Result<TokenResponseModel>.ValidationFailure(validationResult);
        }

        UserModel user = UserModel.CreateUser(request.Dto.Email);
        user.SetPasswordHash(encryptionService.HashPassword(request.Dto.Password));
        await unitOfWork.WithTransaction(async ct =>
        {
            await unitOfWork.Users.AddAsync(user, ct);
            await unitOfWork.SaveChangesAsync(ct);
        }, ct);

        await SendRegistrationEmailAsync(emailService, user.Email, ct);

        var claims = new List<Claim>
        {
            new(ClaimTypes.Sid, user.Id.ToString(), ClaimValueTypes.Sid)
        };

        var (accessToken, accessTokenExpiresAt) = tokenServices.GenerateJwtToken(claims);

        return Result<TokenResponseModel>.Created(new TokenResponseModel()
        {
            AccessToken = accessToken,
            AccessTokenExpiresAt = accessTokenExpiresAt
        });
    }

    private static async Task SendRegistrationEmailAsync(
        IEmailService emailService,
        string userEmail,
        CancellationToken ct)
    {
        var emailBody = """
            <body>
                <main>
                    <section>
                        <p>Dear User,</p>
                        <p>Welcome to <strong>DevInsightForge</strong>. Your account has been created successfully.</p>
                    </section>
                    <section>
                        <p>If you did not create this account, please contact support immediately.</p>
                    </section>
                </main>
                <footer>
                    <p>This is an automated message. Please do not reply.</p>
                </footer>
            </body>
            """;

        try
        {
            await emailService.SendAsync(
                to: userEmail,
                subject: "Welcome to DevInsightForge",
                body: emailBody,
                isHtml: true,
                ct: ct);
        }
        catch
        {
            // Email delivery should not block registration success.
        }
    }
}




