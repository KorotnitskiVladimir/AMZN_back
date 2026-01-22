namespace AMZN.Data.Entities;

public class UserRole
{
    public string Id { get; set; } = null!;
    public string Description { get; set; } = null!;
    public int CanCreate { get; set; }
    public int CanRead { get; set; }
    public int CanUpdate { get; set; }
    public int CanDelete { get; set; }
}