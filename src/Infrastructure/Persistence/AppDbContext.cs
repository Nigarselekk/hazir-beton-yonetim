using HazirBeton.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HazirBeton.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Site> Sites => Set<Site>();
    public DbSet<ConcreteRequest> ConcreteRequests => Set<ConcreteRequest>();
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<Personnel> Personnel => Set<Personnel>();
    public DbSet<VehiclePersonnel> VehiclePersonnel => Set<VehiclePersonnel>();
    public DbSet<ConcreteRequestVehicle> ConcreteRequestVehicles => Set<ConcreteRequestVehicle>();
    public DbSet<User> Users => Set<User>();
    public DbSet<CostEntry> CostEntries => Set<CostEntry>();
    public DbSet<SmsLog> SmsLogs => Set<SmsLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
