using DevInsightForge.Application.Abstructions.DataAccess.Repositories;
using DevInsightForge.Domain.Entities;
using DevInsightForge.Persistence.DataContext;

namespace DevInsightForge.Persistence.DataAccess.Repositories;

public class UserRepository(DatabaseContext dbContext) : GenericRepository<User>(dbContext), IUserRepository
{
}


