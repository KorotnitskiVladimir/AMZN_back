using AMZN.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace AMZN.Data;

public class DataContext: DbContext
{
    public DbSet<UserData> UsersData { get; private set; }
    public DbSet<UserAccess> UsersAccess { get; private set; } 
    public DbSet<UserRole> UserRoles { get; private set; }
    

    public DataContext(DbContextOptions options) : base(options) {}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("AMZN");

        modelBuilder.Entity<UserAccess>()
            .HasIndex(ua => ua.Username)
            .IsUnique();

        modelBuilder.Entity<UserAccess>()
            .HasOne(ua => ua.UserData)
            .WithMany()
            .HasForeignKey(ua => ua.UserId)
            .HasPrincipalKey(ud => ud.Id);

        modelBuilder.Entity<UserAccess>()
            .HasOne(ua => ua.UserRole)
            .WithMany()
            .HasForeignKey(ua => ua.RoleId);

        modelBuilder.Entity<UserRole>().HasData(
            new UserRole()
            {
                Id = "guest", Description = "solely registered user", CanCreate = 0, CanRead = 0, CanUpdate = 0,
                CanDelete = 0
            },
            new UserRole()
            {
                Id = "admin", Description = "full access to DB", CanCreate = 1, CanRead = 1, CanUpdate = 1,
                CanDelete = 1
            },
            new UserRole()
            {
                Id = "moderator", Description = "can block", CanCreate = 0, CanRead = 1, CanUpdate = 0, CanDelete = 1
            },
            new UserRole()
            {
                Id = "editor", Description = "can edit", CanCreate = 0, CanRead = 1, CanUpdate = 1, CanDelete = 0
            });
    }
}