namespace AMZN.Data.Entities;

public class DeletedUser
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    public DateOnly? BirthDate { get; set; }
    public DateTime DeletedAt { get; set; } = DateTime.UtcNow;
}