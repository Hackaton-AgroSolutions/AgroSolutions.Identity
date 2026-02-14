using AgroSolutions.Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace AgroSolutions.Identity.Infrastructure.Persistence;

public class AgroSolutionsIdentityDbContext(DbContextOptions<AgroSolutionsIdentityDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) => modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
}
