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
            var sql = @"
                INSERT INTO Customer
                    (FirstName, LastName,
                    BillingAddressLine1, BillingAddressLine2, BillingAddressLine3, BillingAddressLine4, BillingAddressCity, BillingAddressPostCode, BillingAddressCountry,
                    ShippingAddressLine1, ShippingAddressLine2, ShippingAddressLine3, ShippingAddressLine4, ShippingAddressCity, ShippingAddressPostCode, ShippingAddressCountry)
                VALUES
                    (@FirstName, @LastName,
                    @BillingAddressLine1, @BillingAddressLine2, @BillingAddressLine3, @BillingAddressLine4, @BillingAddressCity, @BillingAddressPostCode, @BillingAddressCountry,
                    @ShippingAddressLine1, @ShippingAddressLine2, @ShippingAddressLine3, @ShippingAddressLine4, @ShippingAddressCity, @ShippingAddressPostCode, @ShippingAddressCountry);
                SELECT last_insert_rowid();";
            var param = new {
                customer.FirstName,
                customer.LastName,
                BillingAddressLine1 = customer.BillingAddress.Line1,
                BillingAddressLine2 = customer.BillingAddress.Line2,
                BillingAddressLine3 = customer.BillingAddress.Line3,
                BillingAddressLine4 = customer.BillingAddress.Line4,
                BillingAddressCity = customer.BillingAddress.City,
                BillingAddressPostCode = customer.BillingAddress.PostCode,
                BillingAddressCountry = customer.BillingAddress.Country,
                ShippingAddressLine1 = customer.ShippingAddress.Line1,
                ShippingAddressLine2 = customer.ShippingAddress.Line2,
                ShippingAddressLine3 = customer.ShippingAddress.Line3,
                ShippingAddressLine4 = customer.ShippingAddress.Line4,
                ShippingAddressCity = customer.ShippingAddress.City,
                ShippingAddressPostCode = customer.ShippingAddress.PostCode,
                ShippingAddressCountry = customer.ShippingAddress.Country,
            };

            using var connection = new SqliteConnection(_customersConnectionString);
            var id = await connection.ExecuteScalarAsync<int>(
                sql,
                param
            );
            return id;
        }

        await _customersDbContext.Customers.AddAsync(customer, cancellationToken);
        await _customersDbContext.SaveChangesAsync(cancellationToken);
        return customer.Id;
    }
}
