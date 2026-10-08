using Learniverse.Application.Interfaces.Common;
using Learniverse.Domain.Common;
using Learniverse.Domain.Entities;
using Microsoft.EntityFrameworkCore.Storage;
using Learniverse.Persistence.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Learniverse.Persistence.Context;

public class AppDbContext
    : IdentityDbContext<ApplicationUser>, IUnitOfWork
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Section> Sections => Set<Section>();
    public DbSet<Lesson> Lessons => Set<Lesson>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();

    public override async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<AuditableEntity>();

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAtUtc = DateTime.UtcNow;
            }
            else if (entry.State == EntityState.Modified)
            {
                // Prevent updating the creation date.
                entry.Property(e => e.CreatedAtUtc).IsModified = false;

                if (!entry.Entity.IsDeleted)
                {
                    entry.Entity.UpdatedAtUtc = DateTime.UtcNow;
                }
            }
        }
         
        return await base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(AppDbContext).Assembly);

        modelBuilder.Entity<Course>()
            .HasQueryFilter(c => !c.IsDeleted);

        modelBuilder.Entity<Section>()
            .HasQueryFilter(s => !s.IsDeleted);

        modelBuilder.Entity<Lesson>()
            .HasQueryFilter(l => !l.IsDeleted);

        modelBuilder.Entity<Category>()
            .HasQueryFilter(c => !c.IsDeleted);
    }
    private IDbContextTransaction? _currentTransaction;

    public async Task BeginTransactionAsync(
        CancellationToken cancellationToken)
    {
        if (_currentTransaction is not null)
            throw new InvalidOperationException(
                "A transaction is already in progress.");

        _currentTransaction =
            await Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(
        CancellationToken cancellationToken)
    {
        if (_currentTransaction is null)
            throw new InvalidOperationException(
                "No active transaction to commit.");

        await _currentTransaction.CommitAsync(cancellationToken);
        await _currentTransaction.DisposeAsync();

        _currentTransaction = null;
    }

    public async Task RollbackTransactionAsync(
        CancellationToken cancellationToken)
    {
        if (_currentTransaction is null)
            throw new InvalidOperationException(
                "No active transaction to rollback.");

        await _currentTransaction.RollbackAsync(cancellationToken);
        await _currentTransaction.DisposeAsync();

        _currentTransaction = null;
    }
}