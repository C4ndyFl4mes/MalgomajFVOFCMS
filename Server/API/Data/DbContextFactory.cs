using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Server.API.Data;

public sealed class AppDbContextFactory() : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql("Host=localhost;Username=postgres;Password=postgres;Database=local_malgomajfvofcms_placeholder")
            .Options;

        return new AppDbContext(options);
    }
}