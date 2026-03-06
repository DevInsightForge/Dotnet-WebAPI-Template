namespace DevInsightForge.Application.DtoModels.Authentication;

public class AuthenticateUserDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class AuthenticateUserDtoValidator : AbstractValidator<AuthenticateUserDto>
{
    public AuthenticateUserDtoValidator()
    {
        RuleFor(dto => dto.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(dto => dto.Password)
            .NotEmpty().WithMessage("Password is required.");
    }
}
