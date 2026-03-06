using DevInsightForge.Application.Abstructions.DataAccess.Repositories;
using DevInsightForge.Domain.Entities.Core;
using DevInsightForge.Infrastructure.Persistence;

namespace DevInsightForge.Infrastructure.DataAccess.Repositories;

public class UserRepository(DatabaseContext dbContext) : GenericRepository<UserModel>(dbContext), IUserRepository
{
}


