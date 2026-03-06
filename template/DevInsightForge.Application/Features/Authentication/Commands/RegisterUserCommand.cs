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
        user.SetPasswordHash(encryptionService.HashPassword(request.Dto.Password));
        await unitOfWork.WithTransaction(async ct =>
        {
            await unitOfWork.Users.AddAsync(user, ct);
            await unitOfWork.SaveChangesAsync(ct);
        }, cancellationToken);

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
}



