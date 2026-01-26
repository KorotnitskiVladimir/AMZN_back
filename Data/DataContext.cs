using AMZN.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace AMZN.Data;

public class DataContext: DbContext
{
    public DbSet<User> Users { get; private set; } = null!;
    public DbSet<UserRefreshToken> UserRefreshTokens { get; private set; } = null!;


    public DataContext(DbContextOptions options) : base(options) {}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("AMZN");


        modelBuilder.Entity<User>(u =>
        {
            u.HasIndex(x => x.Email)
                .IsUnique();

            
            u.Property(x => x.Role)
                .HasConversion<string>();   // храним enum Role как строку в БД (Admin/User), а не как int (0/1)
        });


        modelBuilder.Entity<UserRefreshToken>(u =>
        {
            u.HasIndex(x => x.TokenHash)
                .IsUnique();

            u.HasOne(x => x.User)
                .WithMany(x => x.RefreshTokens)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}