using DevInsightForge.Application.Abstractions.DataAccess;
using DevInsightForge.Application.Contracts.Role;
using DevInsightForge.Application.Results;
using DevInsightForge.Domain.Entities;

namespace DevInsightForge.Application.Features.Roles.Commands;

public sealed record CreateRoleCommand(CreateRoleRequestDto Request) : IRequest<CreateRoleCommand, Task<Result<RoleResponseDto>>>;

internal sealed class CreateRoleCommandHandler(
    IUnitOfWork unitOfWork,
    IValidator<CreateRoleRequestDto> createRoleValidator) : IRequestHandler<CreateRoleCommand, Task<Result<RoleResponseDto>>>
{
    public async Task<Result<RoleResponseDto>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await createRoleValidator.ValidateAsync(request.Request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result<RoleResponseDto>.ValidationFailure(validationResult);
        }

        var role = Role.Create(request.Request.Name);

        await unitOfWork.WithTransaction(async innerCt =>
        {
            await unitOfWork.Roles.AddAsync(role, innerCt);
            await unitOfWork.SaveChangesAsync(innerCt);
        }, cancellationToken);

        return Result<RoleResponseDto>.Created(role.Adapt<RoleResponseDto>());
    }
}
