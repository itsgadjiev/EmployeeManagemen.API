using API.Database.Base;
using API.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Database;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> dbContextOptions) : base(dbContextOptions)
    {
            
    }
    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.Entity is IAuditable)
            {
                var auditable = (IAuditable)entry.Entity;

                if (entry.State == EntityState.Added)
                {
                    auditable.CreatedAt = DateTime.UtcNow;
                    auditable.UpdatedAt = DateTime.UtcNow;
                }
                if (entry.State == EntityState.Modified)
                {
                    auditable.UpdatedAt = DateTime.UtcNow;
                }
            }
        }
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    public DbSet<Employee> Employees { get; set; }
    public DbSet<Department> Departments { get; set; }
}
