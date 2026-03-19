using DevInsightForge.Application.Abstractions.DataAccess;

namespace DevInsightForge.Application.Contracts.User;

public sealed class CreateUserRequestDto
{
    public Guid RoleId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public sealed class CreateUserRequestDtoValidator : AbstractValidator<CreateUserRequestDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateUserRequestDtoValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        RuleFor(dto => dto.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.")
            .MustAsync(BeUniqueEmail).WithMessage("Email is already registered.");

        RuleFor(dto => dto.RoleId)
            .NotEqual(Guid.Empty).WithMessage("Role is required.")
            .MustAsync(RoleExists).WithMessage("Role not found.");

        RuleFor(dto => dto.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
            .Matches(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*\W).+$")
            .WithMessage("Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character.");
    }

    private async Task<bool> BeUniqueEmail(string email, CancellationToken cancellationToken)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();
        return !await _unitOfWork.Users.AnyAsync(u => u.Email == normalizedEmail);
    }

    private async Task<bool> RoleExists(Guid roleId, CancellationToken cancellationToken)
    {
        return await _unitOfWork.Roles.AnyAsync(r => r.Id == roleId);
    }
}



