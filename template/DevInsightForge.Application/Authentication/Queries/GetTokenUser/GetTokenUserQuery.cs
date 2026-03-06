using DevInsightForge.Application.Common.Exceptions;
using DevInsightForge.Application.Common.Interfaces.Core;
using DevInsightForge.Application.Common.Interfaces.DataAccess.Repositories;
using DevInsightForge.Application.Common.ViewModels.User;
using DevInsightForge.Domain.Entities.Core;

namespace DevInsightForge.Application.Authentication.Queries.GetTokenUser;

public sealed record GetTokenUserQuery : IRequest<GetTokenUserQuery, Task<UserResponseModel>>;

internal sealed class GetTokenUserQueryHandler(
    IUserRepository userRepository, 
    IAuthenticatedUser authenticatedUser) : IRequestHandler<GetTokenUserQuery, Task<UserResponseModel>>
{
    public async Task<UserResponseModel> Handle(GetTokenUserQuery request, CancellationToken cancellationToken)
    {
        UserModel? user = await userRepository.GetWhereAsync(u => u.Id.Equals(authenticatedUser.UserId));
        return user is null ? throw new NotFoundException("Token user is not found") : user.Adapt<UserResponseModel>();
    }
}

