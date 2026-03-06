using DevInsightForge.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DevInsightForge.Persistence.DataContext;

public class DatabaseContext(DbContextOptions<DatabaseContext> options) : DbContext(options)
{
    #region Entity DB Set Registrations
    public DbSet<UserModel> Users { get; set; }

    #endregion


    #region EF Core Configuration Overrides
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DatabaseContext).Assembly);

        var foreignKeys = modelBuilder.Model
            .GetEntityTypes()
            .SelectMany(e => e.GetForeignKeys());

        foreach (var relationship in foreignKeys)
        {
            relationship.DeleteBehavior = DeleteBehavior.NoAction;
        }

        base.OnModelCreating(modelBuilder);
    }

    #endregion
}
