using DevInsightForge.Application.Abstractions.DataAccess;

namespace DevInsightForge.Application.Contracts.User;

public sealed class UpdateUserRequestDto
{
    public Guid RoleId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? Password { get; set; }
}

public sealed class UpdateUserRequestDtoValidator : AbstractValidator<UpdateUserRequestDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateUserRequestDtoValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        RuleFor(dto => dto.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(dto => dto.RoleId)
            .NotEqual(Guid.Empty).WithMessage("Role is required.")
            .MustAsync(RoleExists).WithMessage("Role not found.");

        RuleFor(dto => dto.Password)
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
            .Matches(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*\W).+$")
            .WithMessage("Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character.")
            .When(dto => !string.IsNullOrWhiteSpace(dto.Password));
    }

    private async Task<bool> RoleExists(Guid roleId, CancellationToken cancellationToken)
    {
        return await _unitOfWork.Roles.AnyAsync(r => r.Id == roleId);
    }
}



