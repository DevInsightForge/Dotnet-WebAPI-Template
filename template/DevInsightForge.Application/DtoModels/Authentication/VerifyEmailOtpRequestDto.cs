namespace DevInsightForge.Application.DtoModels.Authentication;

public sealed class VerifyEmailOtpRequestDto
{
    public string Email { get; set; } = string.Empty;
    public string Otp { get; set; } = string.Empty;
}

public sealed class VerifyEmailOtpRequestDtoValidator : AbstractValidator<VerifyEmailOtpRequestDto>
{
    public VerifyEmailOtpRequestDtoValidator()
    {
        RuleFor(dto => dto.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(dto => dto.Otp)
            .NotEmpty().WithMessage("OTP is required.")
            .Length(6).WithMessage("OTP must be 6 digits.")
            .Matches("^[0-9]{6}$").WithMessage("OTP must contain only digits.");
    }
}
