using DevInsightForge.Application.Abstructions.DataAccess.Repositories;
using DevInsightForge.Domain.Entities.Core;
using DevInsightForge.Persistence.Persistence;

namespace DevInsightForge.Persistence.DataAccess.Repositories;

public class UserRepository(DatabaseContext dbContext) : GenericRepository<UserModel>(dbContext), IUserRepository
{
}


