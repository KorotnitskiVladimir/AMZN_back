namespace AMZN.Data.Entities;

public class UserData
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    public DateTime? DateOfBirth { get; set; }
    // сюда же потом можно будет добавить сохраненные адреса и методы оплаты, информацию о заказах и прочее
}