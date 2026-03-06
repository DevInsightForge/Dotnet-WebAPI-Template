using DevInsightForge.Application.Abstructions.Core;
using DevInsightForge.Application.Abstructions.DataAccess;
using DevInsightForge.Application.DtoModels.User;
using DevInsightForge.Application.Results;

namespace DevInsightForge.Application.Features.Authentication.Queries;

public sealed record GetCurrentUserQuery : IRequest<GetCurrentUserQuery, Task<Result<UserResponseModel>>>;

internal sealed class GetCurrentUserQueryHandler(
    IUnitOfWork unitOfWork,
    IRequestContextService requestContext) : IRequestHandler<GetCurrentUserQuery, Task<Result<UserResponseModel>>>
{
    public async Task<Result<UserResponseModel>> Handle(GetCurrentUserQuery request, CancellationToken ct)
    {
        if (requestContext.RequestUserId is null)
        {
            return Result<UserResponseModel>.Failure(
                new Error("auth.unauthorized", "Unauthorized request.", ErrorType.Unauthorized));
        }

        var user = await unitOfWork.Users.GetWhereAsync(u => u.Id == requestContext.RequestUserId);
        if (user is null)
        {
            return Result<UserResponseModel>.Failure(
                new Error("user.not_found", "User not found.", ErrorType.NotFound));
        }

        return Result<UserResponseModel>.Success(user.Adapt<UserResponseModel>());
    }
}
