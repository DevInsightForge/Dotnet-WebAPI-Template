using DevInsightForge.Application.Abstructions;
using DevInsightForge.Application.Abstructions.DataAccess;
using DevInsightForge.Application.DtoModels.Authentication;
using DevInsightForge.Application.Results;

namespace DevInsightForge.Application.Features.Authentication.Commands;

public sealed record VerifyEmailOtpCommand(VerifyEmailOtpRequestDto Dto) : IRequest<VerifyEmailOtpCommand, Task<Result>>;

internal sealed class VerifyEmailOtpCommandHandler(
    IOtpService otpService,
    IUnitOfWork unitOfWork,
    IValidator<VerifyEmailOtpRequestDto> validator) : IRequestHandler<VerifyEmailOtpCommand, Task<Result>>
{
    public async Task<Result> Handle(VerifyEmailOtpCommand request, CancellationToken ct)
    {
        var validationResult = await validator.ValidateAsync(request.Dto, ct);
        if (!validationResult.IsValid)
        {
            return Result.ValidationFailure(validationResult);
        }

        var normalizedEmail = request.Dto.Email.Trim().ToLowerInvariant();
        var user = await unitOfWork.Users.GetWhereAsync(u => u.Email == normalizedEmail);
        if (user is null)
        {
            return Result.Failure(new Error("user.not_found", "User not found.", ErrorType.NotFound));
        }

        if (user.IsEmailVerified)
        {
            return Result.Failure(new Error("auth.already_verified", "Email is already verified.", ErrorType.Conflict));
        }

        var isValid = otpService.VerifyEmailVerificationOtp(request.Dto.Email, request.Dto.Otp);
        if (!isValid)
        {
            return Result.Failure(new Error(
                "auth.otp_invalid_or_expired",
                "Invalid or expired OTP code.",
                ErrorType.Validation));
        }

        user.MarkEmailAsVerified();

        await unitOfWork.WithTransaction(async innerCt =>
        {
            await unitOfWork.Users.UpdateAsync(user, innerCt);
            await unitOfWork.SaveChangesAsync(innerCt);
        }, ct);

        return Result.Success();
    }
}
