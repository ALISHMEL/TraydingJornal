using Microsoft.EntityFrameworkCore;
using TradingJournal.Api.Models;


namespace TradingJournal.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Trade> Trades => Set<Trade>();
    public DbSet<Execution> Executions => Set<Execution>();
}