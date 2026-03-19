using System.ComponentModel;

namespace DevInsightForge.Application.Contracts.Authentication;

public sealed class LoginRequestDto
{
    [DefaultValue("admin@system.local")]
    public string Email { get; set; } = string.Empty;

    [DefaultValue("Admin@123")]
    public string Password { get; set; } = string.Empty;
}

public sealed class LoginRequestDtoValidator : AbstractValidator<LoginRequestDto>
{
    public LoginRequestDtoValidator()
    {
        RuleFor(dto => dto.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(dto => dto.Password)
            .NotEmpty().WithMessage("Password is required.");
    }
}



