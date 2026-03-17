namespace DevInsightForge.Application.Contracts.User;

public sealed class UpdateUserRequestDto
{
    public string Email { get; set; } = string.Empty;
    public string? Password { get; set; }
    public bool IsEmailVerified { get; set; } = false;
}

public sealed class UpdateUserRequestDtoValidator : AbstractValidator<UpdateUserRequestDto>
{
    public UpdateUserRequestDtoValidator()
    {
        RuleFor(dto => dto.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(dto => dto.Password)
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
            .Matches(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*\W).+$")
            .WithMessage("Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character.")
            .When(dto => !string.IsNullOrWhiteSpace(dto.Password));
    }
}



