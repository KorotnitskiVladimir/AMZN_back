namespace AMZN.DTOs.Account;

public class DeliveryAddressResponseDto
{
    public Guid Id { get; set; }
    public bool IsDefault { get; set; }
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Address { get; set; } = "";
    public string City { get; set; } = "";
    public string? State { get; set; }
    public string PostalCode { get; set; } = "";
    public string Country { get; set; } = "";
    public string PhoneNumber { get; set; } = "";
}