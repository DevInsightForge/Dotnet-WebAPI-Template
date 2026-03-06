using DevInsightForge.Application.Abstructions.Core;
using DevInsightForge.Application.Abstructions.DataAccess.Repositories;
using DevInsightForge.Application.DtoModels.User;
using DevInsightForge.Domain.Entities.Core;
using DevInsightForge.Application.Results;

namespace DevInsightForge.Application.Features.Authentication.Queries;

public sealed record GetTokenUserQuery : IRequest<GetTokenUserQuery, Task<Result<UserResponseModel>>>;

internal sealed class GetTokenUserQueryHandler(
    IUserRepository userRepository,
    IAuthenticatedUser authenticatedUser) : IRequestHandler<GetTokenUserQuery, Task<Result<UserResponseModel>>>
{
    public async Task<Result<UserResponseModel>> Handle(GetTokenUserQuery request, CancellationToken cancellationToken)
    {
        if (authenticatedUser.UserId is null)
        {
            return Result<UserResponseModel>.Failure(
                new Error("auth.unauthorized", "Unauthorized request.", ErrorType.Unauthorized));
        }

        UserModel? user = await userRepository.GetWhereAsync(u => u.Id.Equals(authenticatedUser.UserId));
        return user is null
            ? Result<UserResponseModel>.Failure(new Error("user.not_found", "Token user is not found.", ErrorType.NotFound))
            : Result<UserResponseModel>.Success(user.Adapt<UserResponseModel>());
    }
}




