using DevInsightForge.Application.Abstractions.DataAccess.Repositories;
using DevInsightForge.Domain.Entities;
using DevInsightForge.Persistence.DataContext;

namespace DevInsightForge.Persistence.DataAccess.Repositories;

public class RoleRepository(DatabaseContext dbContext) : GenericRepository<Role>(dbContext), IRoleRepository
{
}
