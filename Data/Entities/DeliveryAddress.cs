namespace AMZN.Data.Entities;

public class DeliveryAddress
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Address { get; set; } = null!;
    public string City { get; set; } = null!;
    public string PostalCode { get; set; } = null!;
    public string Country { get; set; } = null!;
    public string? State { get; set; }
    public string PhoneNumber { get; set; } = null!;
    public bool IsDefault { get; set; }
    public User User { get; set; } = null!;
}