using DevInsightForge.Application.Abstructions.DataAccess.Repositories;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DevInsightForge.Application.DtoModels.Authentication;

public class RegisterUserDto
{
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [PasswordPropertyText]
    public string Password { get; set; } = string.Empty;
}

public class RegisterUserDtoValidator : AbstractValidator<RegisterUserDto>
{
    private readonly IUserRepository _userRepository;

    public RegisterUserDtoValidator(IUserRepository userRepository)
    {
        _userRepository = userRepository;

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

    private async Task<bool> BeUniqueEmail(string email, CancellationToken cancellationToken)
    {
        return !await _userRepository.AnyAsync(u => u.Email == email);
    }
}

