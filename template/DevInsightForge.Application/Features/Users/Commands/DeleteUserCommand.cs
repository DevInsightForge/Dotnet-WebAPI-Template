using DevInsightForge.Application.Abstructions.DataAccess;
using DevInsightForge.Application.Results;

namespace DevInsightForge.Application.Features.Users.Commands;

public sealed record DeleteUserCommand(Guid UserId) : IRequest<DeleteUserCommand, Task<Result>>;

internal sealed class DeleteUserCommandHandler(
    IUnitOfWork unitOfWork) : IRequestHandler<DeleteUserCommand, Task<Result>>
{
    public async Task<Result> Handle(DeleteUserCommand request, CancellationToken ct)
    {
        var user = await unitOfWork.Users.GetByIdAsync(request.UserId);
        if (user is null)
        {
            return Result.Failure(
                new Error("user.not_found", "User not found.", ErrorType.NotFound));
        }

        user.MarkAsDeleted();

        await unitOfWork.WithTransaction(async innerCt =>
        {
            await unitOfWork.Users.UpdateAsync(user, innerCt);
            await unitOfWork.SaveChangesAsync(innerCt);
        }, ct);

        return Result.Success();
    }
}
