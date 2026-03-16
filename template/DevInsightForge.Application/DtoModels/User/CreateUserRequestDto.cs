using DevInsightForge.Application.Abstructions.DataAccess;

namespace DevInsightForge.Application.DtoModels.User;

public sealed class CreateUserRequestDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool IsEmailVerified { get; set; } = false;
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

        RuleFor(dto => dto.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
            .Matches(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*\W).+$")
            .WithMessage("Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character.");
    }

    private async Task<bool> BeUniqueEmail(string email, CancellationToken ct)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();
        return !await _unitOfWork.Users.AnyAsync(u => u.Email == normalizedEmail);
    }
}
