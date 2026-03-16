using DevInsightForge.Application.Abstructions.DataAccess;
using DevInsightForge.Application.DtoModels.User;
using DevInsightForge.Application.Results;

namespace DevInsightForge.Application.Features.Users.Queries;

public sealed record GetUserByIdQuery(Guid UserId) : IRequest<GetUserByIdQuery, Task<Result<UserResponseModel>>>;

internal sealed class GetUserByIdQueryHandler(
    IUnitOfWork unitOfWork) : IRequestHandler<GetUserByIdQuery, Task<Result<UserResponseModel>>>
{
    public async Task<Result<UserResponseModel>> Handle(GetUserByIdQuery request, CancellationToken ct)
    {
        var user = await unitOfWork.Users.GetByIdAsync(request.UserId);
        if (user is null)
        {
            return Result<UserResponseModel>.Failure(
                new Error("user.not_found", "User not found.", ErrorType.NotFound));
        }

        return Result<UserResponseModel>.Success(user.Adapt<UserResponseModel>());
    }
}
