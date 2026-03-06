using DevInsightForge.Application.Exceptions;
using DevInsightForge.Application.DtoModels.Authentication;
using DevInsightForge.Application.Abstructions;
using DevInsightForge.Application.Abstructions.DataAccess;
using DevInsightForge.Application.Abstructions.DataAccess.Repositories;

namespace DevInsightForge.Application.Features.Authentication.Commands;

public sealed record AuthenticateUserCommand(AuthenticateUserDto Dto) : IRequest<AuthenticateUserCommand, Task<TokenResponseModel>>;

internal sealed class AuthenticateUserCommandHandler(
    IPasswordHashService passwordHashService,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    ITokenService tokenServices) : IRequestHandler<AuthenticateUserCommand, Task<TokenResponseModel>>
{
    public async Task<TokenResponseModel> Handle(AuthenticateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetWhereAsync(u => u.NormalizedEmail.Equals(request.Dto.Email));

        if (user is null || !passwordHashService.VerifyHashedPassword(user, user.PasswordHash, request.Dto.Password))
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



