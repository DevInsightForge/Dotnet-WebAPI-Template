using DevInsightForge.Application.Abstractions.DataAccess;
using DevInsightForge.Application.Results;

namespace DevInsightForge.Application.Features.Roles.Commands;

public sealed record DeleteRoleCommand(Guid RoleId) : IRequest<DeleteRoleCommand, Task<Result>>;

internal sealed class DeleteRoleCommandHandler(
    IUnitOfWork unitOfWork) : IRequestHandler<DeleteRoleCommand, Task<Result>>
{
    public async Task<Result> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await unitOfWork.Roles.GetByIdAsync(request.RoleId);
        if (role is null)
        {
            return Result.Failure(
                new Error("role.not_found", "Role not found.", ErrorType.NotFound));
        }

        var hasUsers = await unitOfWork.Users.AnyAsync(u => u.RoleId == role.Id);
        if (hasUsers)
        {
            return Result.Failure(
                new Error("role.in_use", "Role cannot be deleted because it is assigned to one or more users.", ErrorType.Conflict));
        }

        role.MarkAsDeleted();

        await unitOfWork.WithTransaction(async innerCt =>
        {
            await unitOfWork.Roles.UpdateAsync(role, innerCt);
            await unitOfWork.SaveChangesAsync(innerCt);
        }, cancellationToken);

        return Result.Success();
    }
}
