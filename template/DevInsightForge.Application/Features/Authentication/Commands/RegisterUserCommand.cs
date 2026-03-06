using DevInsightForge.Application.DtoModels.Authentication;
using DevInsightForge.Application.Abstructions;
using DevInsightForge.Application.Abstructions.DataAccess;
using DevInsightForge.Application.Abstructions.DataAccess.Repositories;
using DevInsightForge.Domain.Entities.Core;
using DevInsightForge.Application.Results;

namespace DevInsightForge.Application.Features.Authentication.Commands;

public sealed record RegisterUserCommand(RegisterUserDto Dto) : IRequest<RegisterUserCommand, Task<Result<TokenResponseModel>>>;

internal sealed class RegisterUserCommandHandler(
    IUserRepository userRepository,
    IPasswordHashService passwordHashService,
    IUnitOfWork unitOfWork,
    ITokenService tokenServices,
    IValidator<RegisterUserDto> registerUserDtoValidator) : IRequestHandler<RegisterUserCommand, Task<Result<TokenResponseModel>>>
{
    public async Task<Result<TokenResponseModel>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await registerUserDtoValidator.ValidateAsync(request.Dto, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result<TokenResponseModel>.ValidationFailure(validationResult);
        }

        UserModel user = UserModel.CreateUser(request.Dto.Email);
        user.SetPasswordHash(passwordHashService.HashPassword(user, request.Dto.Password));

        await userRepository.AddAsync(user, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var (accessToken, accessTokenExpiresAt) = tokenServices.GenerateJwtToken(user.Id);

        return Result<TokenResponseModel>.Created(new TokenResponseModel()
        {
            AccessToken = accessToken,
            AccessTokenExpiresAt = accessTokenExpiresAt
        });
    }
}



