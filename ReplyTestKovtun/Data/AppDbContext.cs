using Microsoft.EntityFrameworkCore;
using ReplyTestKovtun.Data.Configurations;
using ReplyTestKovtun.Models;

namespace ReplyTestKovtun.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Contract> Contracts => Set<Contract>();
    public DbSet<ContractDynamicField> ContractDynamicFields => Set<ContractDynamicField>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ContractConfiguration());
        modelBuilder.ApplyConfiguration(new ContractDynamicFieldConfiguration());
    }
}
