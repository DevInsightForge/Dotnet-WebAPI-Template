using DevInsightForge.Application.Common.Interfaces;
using DevInsightForge.Application.Common.Interfaces.DataAccess;
using DevInsightForge.Application.Common.Interfaces.DataAccess.Repositories;
using DevInsightForge.Application.Common.ViewModels.Authentication;
using DevInsightForge.Domain.Entities.Core;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DevInsightForge.Application.Authentication.Commands.RegisterUser;

public sealed record RegisterUserCommand : IRequest<RegisterUserCommand, Task<TokenResponseModel>>
{
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [PasswordPropertyText]
    public string Password { get; set; } = string.Empty;
}

internal sealed class RegisterUserCommandHandler(
    IUserRepository userRepository,
    IPasswordHashService passwordHashService,
    IJwtTokenLifetime jwtTokenLifetime,
    IUnitOfWork unitOfWork,
    ITokenService tokenServices) : IRequestHandler<RegisterUserCommand, Task<TokenResponseModel>>
{
    public async Task<TokenResponseModel> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        UserModel user = UserModel.CreateUser(request.Email);
        user.SetPasswordHash(passwordHashService.HashPassword(user, request.Password));

        await userRepository.AddAsync(user, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var jwtExpiryDate = DateTime.UtcNow.AddMinutes(jwtTokenLifetime.AccessTokenExpirationInMinutes);
        var refreshExpiryDate = DateTime.UtcNow.AddMinutes(jwtTokenLifetime.RefreshTokenExpirationInMinutes);

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

