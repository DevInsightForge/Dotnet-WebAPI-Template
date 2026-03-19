using DevInsightForge.Application.Abstractions.DataAccess;
using DevInsightForge.Application.Abstractions.InternalServices;
using DevInsightForge.Application.Contracts.User;
using DevInsightForge.Application.Results;

namespace DevInsightForge.Application.Features.Authentication.Queries;

public sealed record GetCurrentUserQuery : IRequest<GetCurrentUserQuery, Task<Result<UserResponseDto>>>;

internal sealed class GetCurrentUserQueryHandler(
    IUnitOfWork unitOfWork,
    IRequestContextService requestContext) : IRequestHandler<GetCurrentUserQuery, Task<Result<UserResponseDto>>>
{
    public async Task<Result<UserResponseDto>> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        if (requestContext.RequestUserId is null)
        {
            return Result<UserResponseDto>.Failure(
                new Error("auth.unauthorized", "Unauthorized request.", ErrorType.Unauthorized));
        }

        var user = await unitOfWork.Users.GetWhereAsync(u => u.Id == requestContext.RequestUserId, u => u.Role!);
        if (user is null)
        {
            return Result<UserResponseDto>.Failure(
                new Error("user.not_found", "User not found.", ErrorType.NotFound));
        }

        return Result<UserResponseDto>.Success(user.Adapt<UserResponseDto>());
    }
}



