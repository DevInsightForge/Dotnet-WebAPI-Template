using Microsoft.EntityFrameworkCore;

namespace DevInsightForge.Persistence.DataContext;

public class DatabaseContext(DbContextOptions<DatabaseContext> options) : DbContext(options)
{
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
}

