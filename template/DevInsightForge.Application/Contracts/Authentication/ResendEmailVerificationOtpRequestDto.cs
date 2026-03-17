namespace DevInsightForge.Application.Contracts.Authentication;

public sealed class ResendEmailVerificationOtpRequestDto
{
    public string Email { get; set; } = string.Empty;
}

public sealed class ResendEmailVerificationOtpRequestDtoValidator : AbstractValidator<ResendEmailVerificationOtpRequestDto>
{
    public ResendEmailVerificationOtpRequestDtoValidator()
    {
        RuleFor(dto => dto.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");
    }
}



