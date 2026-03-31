namespace AMZN.DTOs.Account;

public class PaymentMethodResponseDto
{
    public Guid Id { get; set; }
    public bool IsDefault { get; set; }
    public string HolderFirstName { get; set; } = "";
    public string HolderLastName { get; set; } = "";
    public string CardNumber { get; set; } = "";
    public DateOnly ExpirationDate { get; set; }
}