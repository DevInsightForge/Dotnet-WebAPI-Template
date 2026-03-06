using DevInsightForge.Application.Common.Configurations.Settings;
using DevInsightForge.Application.Common.Exceptions;
using DevInsightForge.Application.Common.Interfaces;
using DevInsightForge.Application.Common.Interfaces.DataAccess;
using DevInsightForge.Application.Common.Interfaces.DataAccess.Repositories;
using DevInsightForge.Application.Common.ViewModels.Authentication;
using DevInsightForge.Domain.Entities.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
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
    IPasswordHasher<UserModel> passwordHasher,
    IOptions<JwtSettings> jwtSettings,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    ITokenService tokenServices) : IRequestHandler<AuthenticateUserCommand, Task<TokenResponseModel>>
{
    private readonly JwtSettings _jwtSettings = jwtSettings.Value;

    public async Task<TokenResponseModel> Handle(AuthenticateUserCommand request, CancellationToken cancellationToken)
    {
        UserModel? user = await userRepository.GetWhereAsync(u => u.NormalizedEmail.Equals(request.Email));

        if (user is null || passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password) != PasswordVerificationResult.Success)
        {
            throw new BadRequestException("Invalid Credentials!");
        }

        user.UpdateLastLogin();
        await userRepository.UpdateAsync(user, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var jwtExpiryDate = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationInMinutes);
        var refreshExpiryDate = DateTime.UtcNow.AddMinutes(_jwtSettings.RefreshTokenExpirationInMinutes);

        string accessToken = tokenServices.GenerateJwtToken(user.Id, jwtExpiryDate);
        string refreshToken = await tokenServices.GenerateRefreshTokenAsync(user.Id, refreshExpiryDate);

        return new TokenResponseModel()
        {
            RefreshToken = refreshToken,
            RefreshTokenExpiresAt = refreshExpiryDate,
            AccessToken = accessToken,
            AccessTokenExpiresAt = jwtExpiryDate
        };
    }
}

