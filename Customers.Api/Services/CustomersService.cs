using Customers.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Customers.Api;

public interface ICustomersService
{
    Task<Customer?> GetCustomerAsync(int id, CancellationToken cancellationToken = default);
    Task<int> CreateCustomerAsync(Customer customer, CancellationToken cancellationToken = default);
}

public class CustomersService : ICustomersService
{
    private readonly CustomersDbContext _customersDbContext;

    public CustomersService(CustomersDbContext customersDbContext)
    {
        _customersDbContext = customersDbContext;
    }

    public async Task<Customer?> GetCustomerAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _customersDbContext.Customers
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<int> CreateCustomerAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        await _customersDbContext.Customers.AddAsync(customer, cancellationToken);
        await _customersDbContext.SaveChangesAsync(cancellationToken);
        return customer.Id;
    }
}
