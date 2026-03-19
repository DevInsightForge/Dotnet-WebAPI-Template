using DevInsightForge.Application.Abstractions.DataAccess;
using DevInsightForge.Application.Contracts.User;
using DevInsightForge.Application.Results;

namespace DevInsightForge.Application.Features.Users.Queries;

public sealed record GetUserByIdQuery(Guid UserId) : IRequest<GetUserByIdQuery, Task<Result<UserResponseDto>>>;

internal sealed class GetUserByIdQueryHandler(
    IUnitOfWork unitOfWork) : IRequestHandler<GetUserByIdQuery, Task<Result<UserResponseDto>>>
{
    public async Task<Result<UserResponseDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await unitOfWork.Users.GetWhereAsync(u => u.Id == request.UserId, u => u.Role!);
        if (user is null)
        {
            return Result<UserResponseDto>.Failure(
                new Error("user.not_found", "User not found.", ErrorType.NotFound));
        }

        return Result<UserResponseDto>.Success(user.Adapt<UserResponseDto>());
    }
}



