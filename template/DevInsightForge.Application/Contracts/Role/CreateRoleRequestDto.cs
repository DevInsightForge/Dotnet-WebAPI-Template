using DevInsightForge.Application.Abstractions.DataAccess;

namespace DevInsightForge.Application.Contracts.Role;

public sealed class CreateRoleRequestDto
{
    public string Name { get; set; } = string.Empty;
}

public sealed class CreateRoleRequestDtoValidator : AbstractValidator<CreateRoleRequestDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateRoleRequestDtoValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        RuleFor(dto => dto.Name)
            .NotEmpty().WithMessage("Role name is required.")
            .MaximumLength(100).WithMessage("Role name must not exceed 100 characters.")
            .MustAsync(BeUniqueRoleName).WithMessage("Role name already exists.");
    }

    private async Task<bool> BeUniqueRoleName(string name, CancellationToken cancellationToken)
    {
        var normalizedName = name.Trim().ToLowerInvariant();
        return !await _unitOfWork.Roles.AnyAsync(r => r.Name.ToLower() == normalizedName);
    }
}
