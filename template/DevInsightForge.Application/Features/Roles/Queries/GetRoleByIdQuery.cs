using DevInsightForge.Application.Abstractions.DataAccess;
using DevInsightForge.Application.Contracts.Role;
using DevInsightForge.Application.Results;

namespace DevInsightForge.Application.Features.Roles.Queries;

public sealed record GetRoleByIdQuery(Guid RoleId) : IRequest<GetRoleByIdQuery, Task<Result<RoleResponseDto>>>;

internal sealed class GetRoleByIdQueryHandler(
    IUnitOfWork unitOfWork) : IRequestHandler<GetRoleByIdQuery, Task<Result<RoleResponseDto>>>
{
    public async Task<Result<RoleResponseDto>> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
    {
        var role = await unitOfWork.Roles.GetByIdAsync(request.RoleId);
        if (role is null)
        {
            return Result<RoleResponseDto>.Failure(
                new Error("role.not_found", "Role not found.", ErrorType.NotFound));
        }

        return Result<RoleResponseDto>.Success(role.Adapt<RoleResponseDto>());
    }
}
