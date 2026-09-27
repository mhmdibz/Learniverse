using Learniverse.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Learniverse.Persistence.Context;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

        optionsBuilder.UseSqlServer(
     "Server=127.0.0.1,1433;Database=LearniverseDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True");
        return new AppDbContext(optionsBuilder.Options);
    }
}