using Customers.Api.Models;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Customers.Api;

public interface ICustomersService
{
    Task<Customer?> GetCustomerAsync(int id, bool useDapper = false, CancellationToken cancellationToken = default);
    Task<int> CreateCustomerAsync(Customer customer, bool useDapper = false, CancellationToken cancellationToken = default);
}

public class CustomersService : ICustomersService
{
    private readonly CustomersDbContext _customersDbContext;
    private readonly string _customersConnectionString;

    public CustomersService(CustomersDbContext customersDbContext, IConfiguration configuration)
    {
        _customersDbContext = customersDbContext;
        _customersConnectionString = configuration.GetConnectionString("CustomersConnectionString")!;
    }

    public async Task<Customer?> GetCustomerAsync(int id, bool useDapper = false, CancellationToken cancellationToken = default)
    {
        if (useDapper)
        {
            using var connection = new SqliteConnection(_customersConnectionString);
            var customer = await connection.QueryAsync<Customer?, Address, Address, Customer?>(
                "SELECT * FROM Customer WHERE Id = @id",
                (customer, billingAddress, shippingAddress) => {
                    customer.BillingAddress = billingAddress;
                    customer.ShippingAddress = shippingAddress;
                    return customer;
                },
                new { id },
                splitOn: "BillingAddressLine1, ShippingAddressLine1");

            return customer.FirstOrDefault();
        }

        return await _customersDbContext.Customers
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<int> CreateCustomerAsync(Customer customer, bool useDapper = false, CancellationToken cancellationToken = default)
    {
        if (useDapper)
        {

        }

        await _customersDbContext.Customers.AddAsync(customer, cancellationToken);
        await _customersDbContext.SaveChangesAsync(cancellationToken);
        return customer.Id;
    }
}
