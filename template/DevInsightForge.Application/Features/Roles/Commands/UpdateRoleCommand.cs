using DevInsightForge.Application.Abstractions.DataAccess;
using DevInsightForge.Application.Contracts.Role;
using DevInsightForge.Application.Results;

namespace DevInsightForge.Application.Features.Roles.Commands;

public sealed record UpdateRoleCommand(Guid RoleId, UpdateRoleRequestDto Request) : IRequest<UpdateRoleCommand, Task<Result<RoleResponseDto>>>;

internal sealed class UpdateRoleCommandHandler(
    IUnitOfWork unitOfWork,
    IValidator<UpdateRoleRequestDto> updateRoleValidator) : IRequestHandler<UpdateRoleCommand, Task<Result<RoleResponseDto>>>
{
    public async Task<Result<RoleResponseDto>> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await updateRoleValidator.ValidateAsync(request.Request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result<RoleResponseDto>.ValidationFailure(validationResult);
        }

        var role = await unitOfWork.Roles.GetByIdAsync(request.RoleId);
        if (role is null)
        {
            return Result<RoleResponseDto>.Failure(
                new Error("role.not_found", "Role not found.", ErrorType.NotFound));
        }

        var normalizedName = request.Request.Name.Trim().ToLowerInvariant();
        var nameInUse = await unitOfWork.Roles.AnyAsync(r => r.Id != role.Id && r.Name.ToLower() == normalizedName);
        if (nameInUse)
        {
            return Result<RoleResponseDto>.Failure(
                new Error("role.name_conflict", "Role name already exists.", ErrorType.Conflict));
        }

        role.SetName(request.Request.Name);

        await unitOfWork.WithTransaction(async innerCt =>
        {
            await unitOfWork.Roles.UpdateAsync(role, innerCt);
            await unitOfWork.SaveChangesAsync(innerCt);
        }, cancellationToken);

        return Result<RoleResponseDto>.Success(role.Adapt<RoleResponseDto>());
    }
}
