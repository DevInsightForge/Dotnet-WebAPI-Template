using DevInsightForge.Application.DtoModels.Authentication;
using DevInsightForge.Application.Abstructions;
using DevInsightForge.Application.Abstructions.DataAccess;
using DevInsightForge.Application.Abstructions.DataAccess.Repositories;
using DevInsightForge.Application.Results;

namespace DevInsightForge.Application.Features.Authentication.Commands;

public sealed record AuthenticateUserCommand(AuthenticateUserDto Dto) : IRequest<AuthenticateUserCommand, Task<Result<TokenResponseModel>>>;

internal sealed class AuthenticateUserCommandHandler(
    IPasswordHashService passwordHashService,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    ITokenService tokenServices,
    IValidator<AuthenticateUserDto> authenticateUserDtoValidator) : IRequestHandler<AuthenticateUserCommand, Task<Result<TokenResponseModel>>>
{
    public async Task<Result<TokenResponseModel>> Handle(AuthenticateUserCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await authenticateUserDtoValidator.ValidateAsync(request.Dto, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result<TokenResponseModel>.ValidationFailure(validationResult);
        }

        var normalizedEmail = request.Dto.Email.ToUpperInvariant();
        var user = await userRepository.GetWhereAsync(u => u.NormalizedEmail.Equals(normalizedEmail));

        if (user is null || !passwordHashService.VerifyHashedPassword(user, user.PasswordHash, request.Dto.Password))
        {
            return Result<TokenResponseModel>.Failure(
                new Error("auth.invalid_credentials", "Invalid credentials.", ErrorType.Unauthorized));
        }

        user.UpdateLastLogin();
        await userRepository.UpdateAsync(user, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var (accessToken, accessTokenExpiresAt) = tokenServices.GenerateJwtToken(user.Id);

        return Result<TokenResponseModel>.Success(new TokenResponseModel()
        {
            AccessToken = accessToken,
            AccessTokenExpiresAt = accessTokenExpiresAt
        });
    }
}



