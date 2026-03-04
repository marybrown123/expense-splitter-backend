using ExpenseSplitter.Application.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExpenseSplitter.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
}