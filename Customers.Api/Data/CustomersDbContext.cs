using Customers.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Customers.Api;

public class CustomersDbContext : DbContext
{
    public DbSet<Customer> Customers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        new CustomerEntityTypeConfiguration().Configure(modelBuilder.Entity<Customer>());
    }

    public CustomersDbContext(DbContextOptions<CustomersDbContext> options)
        : base(options)
    { }
}
