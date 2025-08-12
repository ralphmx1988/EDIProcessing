using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EDIProcessingApp.Core.Entities;

namespace EDIProcessingApp.Infrastructure.Data;

public class EDIProcessingDbContext : DbContext
{
    public EDIProcessingDbContext(DbContextOptions<EDIProcessingDbContext> options) : base(options)
    {
    }

    public DbSet<Account> Accounts { get; set; }
    public DbSet<AccountType> AccountTypes { get; set; }
    public DbSet<AccountConfiguration> AccountConfigurations { get; set; }
    public DbSet<EdiFile> EdiFiles { get; set; }
    public DbSet<EdiTransactionType> EdiTransactionTypes { get; set; }
    public DbSet<Transaction> Transactions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all configurations from the current assembly
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Set default schema
        modelBuilder.HasDefaultSchema("edi");

        // Global query filters for soft delete (if needed in future)
        // modelBuilder.Entity<Account>().HasQueryFilter(x => !x.IsDeleted);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Update timestamps automatically
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is Account || e.Entity is AccountConfiguration || e.Entity is AccountType)
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                if (entry.Entity is Account account)
                {
                    account.CreatedAt = DateTime.UtcNow;
                    account.UpdatedAt = DateTime.UtcNow;
                }
                else if (entry.Entity is AccountConfiguration config)
                {
                    config.CreatedAt = DateTime.UtcNow;
                    config.UpdatedAt = DateTime.UtcNow;
                }
                else if (entry.Entity is AccountType accountType)
                {
                    accountType.CreatedDate = DateTime.UtcNow;
                }
            }
            else if (entry.State == EntityState.Modified)
            {
                if (entry.Entity is Account account)
                {
                    account.UpdatedAt = DateTime.UtcNow;
                }
                else if (entry.Entity is AccountConfiguration config)
                {
                    config.UpdatedAt = DateTime.UtcNow;
                }
                else if (entry.Entity is AccountType accountType)
                {
                    accountType.UpdatedDate = DateTime.UtcNow;
                }
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
