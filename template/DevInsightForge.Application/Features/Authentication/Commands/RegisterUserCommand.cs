using DevInsightForge.Application.DtoModels.Authentication;
using DevInsightForge.Application.Abstructions;
using DevInsightForge.Application.Abstructions.DataAccess;
using DevInsightForge.Application.Abstructions.DataAccess.Repositories;
using DevInsightForge.Domain.Entities.Core;

namespace DevInsightForge.Application.Features.Authentication.Commands;

public sealed record RegisterUserCommand(RegisterUserDto Dto) : IRequest<RegisterUserCommand, Task<TokenResponseModel>>;

internal sealed class RegisterUserCommandHandler(
    IUserRepository userRepository,
    IPasswordHashService passwordHashService,
    IUnitOfWork unitOfWork,
    ITokenService tokenServices) : IRequestHandler<RegisterUserCommand, Task<TokenResponseModel>>
{
    public async Task<TokenResponseModel> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        UserModel user = UserModel.CreateUser(request.Dto.Email);
        user.SetPasswordHash(passwordHashService.HashPassword(user, request.Dto.Password));

        await userRepository.AddAsync(user, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var (accessToken, accessTokenExpiresAt) = tokenServices.GenerateJwtToken(user.Id);

        return new TokenResponseModel()
        {
            AccessToken = accessToken,
            AccessTokenExpiresAt = accessTokenExpiresAt
        };
    }
}



