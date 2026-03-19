namespace DevInsightForge.Application.Contracts.Role;

public sealed class UpdateRoleRequestDto
{
    public string Name { get; set; } = string.Empty;
}

public sealed class UpdateRoleRequestDtoValidator : AbstractValidator<UpdateRoleRequestDto>
{
    public UpdateRoleRequestDtoValidator()
    {
        RuleFor(dto => dto.Name)
            .NotEmpty().WithMessage("Role name is required.")
            .MaximumLength(100).WithMessage("Role name must not exceed 100 characters.");
    }
}
