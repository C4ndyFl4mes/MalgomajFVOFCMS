using Microsoft.EntityFrameworkCore;

namespace Server.API.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    // Will add code later.
}