namespace Customers.Api.Models;

public class Customer
{
    public long Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public Address BillingAddress { get; set; }
    public Address ShippingAddress { get; set; }
}