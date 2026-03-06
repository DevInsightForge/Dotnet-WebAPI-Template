using DevInsightForge.Application.Common.Exceptions;
using DevInsightForge.Application.Common.Interfaces;
using DevInsightForge.Application.Common.Interfaces.DataAccess;
using DevInsightForge.Application.Common.Interfaces.DataAccess.Repositories;
using DevInsightForge.Application.Common.ViewModels.Authentication;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DevInsightForge.Application.Authentication.Commands.AuthenticateUser;

public sealed record AuthenticateUserCommand : IRequest<AuthenticateUserCommand, Task<TokenResponseModel>>
{
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [PasswordPropertyText]
    public string Password { get; set; } = string.Empty;
}

internal sealed class AuthenticateUserCommandHandler(
    IPasswordHashService passwordHashService,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    ITokenService tokenServices) : IRequestHandler<AuthenticateUserCommand, Task<TokenResponseModel>>
{
    public async Task<TokenResponseModel> Handle(AuthenticateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetWhereAsync(u => u.NormalizedEmail.Equals(request.Email));

        if (user is null || !passwordHashService.VerifyHashedPassword(user, user.PasswordHash, request.Password))
        {
            throw new BadRequestException("Invalid Credentials!");
        }

        user.UpdateLastLogin();
        await userRepository.UpdateAsync(user, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var (accessToken, accessTokenExpiresAt) = tokenServices.GenerateJwtToken(user.Id);

        return new TokenResponseModel()
        {
            AccessToken = accessToken,
            AccessTokenExpiresAt = accessTokenExpiresAt
        };
    }
}

