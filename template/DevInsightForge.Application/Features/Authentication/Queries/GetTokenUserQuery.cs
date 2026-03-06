using DevInsightForge.Application.Exceptions;
using DevInsightForge.Application.Abstructions.Core;
using DevInsightForge.Application.Abstructions.DataAccess.Repositories;
using DevInsightForge.Application.DtoModels.User;
using DevInsightForge.Domain.Entities.Core;

namespace DevInsightForge.Application.Features.Authentication.Queries;

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




