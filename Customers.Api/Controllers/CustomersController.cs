using Customers.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Customers.Api;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly ICustomersService _customersService;

    public CustomersController(ICustomersService customersService)
    {
        _customersService = customersService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCustomerAsync(int id, [FromQuery] bool useDapper)
    {
        var customer = await _customersService.GetCustomerAsync(id, useDapper);
        return Ok(customer);
    }

    [HttpPost]
    public async Task<IActionResult> CreateCustomerAsync([FromBody] CustomerRequest request)
    {
        var customer = new Customer
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            BillingAddress = new Address
            {
                Line1 = request.BillingAddress.Line1,
                Line2 = request.BillingAddress.Line2,
                Line3 = request.BillingAddress.Line3,
                Line4 = request.BillingAddress.Line4,
                City = request.BillingAddress.City,
                PostCode = request.BillingAddress.PostCode,
                Country = request.BillingAddress.Country,
            },
            ShippingAddress = new Address
            {
                Line1 = request.ShippingAddress.Line1,
                Line2 = request.ShippingAddress.Line2,
                Line3 = request.ShippingAddress.Line3,
                Line4 = request.ShippingAddress.Line4,
                City = request.ShippingAddress.City,
                PostCode = request.ShippingAddress.PostCode,
                Country = request.ShippingAddress.Country,
            },
        };
        var id = await _customersService.CreateCustomerAsync(customer);
        return Ok(new CustomerResponse(id));
    }
}

public record CustomerRequest(
    string FirstName,
    string LastName,
    CustomerRequestAddress BillingAddress,
    CustomerRequestAddress ShippingAddress);

public record CustomerRequestAddress(
    string Line1,
    string? Line2,
    string? Line3,
    string? Line4,
    string City,
    string PostCode,
    string Country);

public record CustomerResponse(int id);