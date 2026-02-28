///
/// Admin - CRUD
/// Moderator - RD
/// Editor - RU
/// 

using AMZN.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Action = AMZN.Data.Entities.Action;

namespace AMZN.Data;

public class DataContext: DbContext
{
    public DbSet<User> Users { get; private set; } = null!;
    public DbSet<UserRefreshToken> UserRefreshTokens { get; private set; } = null!;

    public DbSet<Product> Products { get; private set; } = null!;
    public DbSet<ProductImage> ProductImages { get; private set; } = null!;
    public DbSet<ProductRating> ProductRatings { get; private set; } = null!;

    public DbSet<Category> Categories { get; private set; } = null!;
    public DbSet<Action> Actions { get; private set; } = null!;
    public DbSet<ProductAction> ProductActions { get; private set; } = null!;
    
    public DbSet<CategoryAction> CategoryActions { get; private set; } = null!;


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


        modelBuilder.Entity<Product>(p =>
        {
            p.HasIndex(x => x.CategoryId);

            p.Property(x => x.Title)
                .HasMaxLength(256)
                .IsRequired();

            p.Property(x => x.PrimaryImageUrl)
                .HasMaxLength(2048)
                .IsRequired();

            p.Property(x => x.CurrentPrice)
                .HasColumnType("decimal(18,2)");

            p.Property(x => x.OriginalPrice)
                .HasColumnType("decimal(18,2)");

            p.HasOne(x => x.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            p.ToTable(t =>
            {
                t.HasCheckConstraint("CK_Product_CurrentPrice", "[CurrentPrice] >= 0");

                t.HasCheckConstraint("CK_Product_OriginalPrice",
                    "[OriginalPrice] IS NULL OR [OriginalPrice] >= [CurrentPrice]");

                t.HasCheckConstraint("CK_Product_RatingSum", "[RatingSum] >= 0");
                t.HasCheckConstraint("CK_Product_RatingCount", "[RatingCount] >= 0");
            });
        });

        modelBuilder.Entity<ProductImage>(i =>
        {
            i.HasIndex(x => x.ProductId);

            i.Property(x => x.Url)
                .HasMaxLength(2048)
                .IsRequired();

            i.HasOne(x => x.Product)
                .WithMany(p => p.Images)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            i.HasIndex(x => new { x.ProductId, x.SortOrder })
                .IsUnique();

            i.ToTable(t =>
            {
                t.HasCheckConstraint("CK_ProductImage_SortOrder", "[SortOrder] >= 0");
            });
        });


        modelBuilder.Entity<ProductRating>(r =>
        {
            r.HasIndex(x => new { x.ProductId, x.UserId })
                .IsUnique();

            r.Property(x => x.Value)
                .IsRequired();

            r.HasOne(x => x.Product)
                .WithMany(p => p.Ratings)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            r.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            r.ToTable(t =>
            {
                t.HasCheckConstraint("CK_ProductRating_Value", "[Value] >= 1 AND [Value] <= 5");
            });
        });



        modelBuilder.Entity<Category>()
            .HasIndex(c => c.Name)
            .IsUnique();

        modelBuilder.Entity<Category>()
            .HasOne(c => c.ParentCategory)
            .WithMany()
            .HasForeignKey(c => c.ParentId);
        /*
        modelBuilder.Entity<Action>()
            .HasOne(a => a.Product)
            .WithMany()
            .HasForeignKey(a => a.ProductId)
            .HasPrincipalKey(p => p.Id);
        */
        
        modelBuilder.Entity<ProductAction>()
            .HasOne(pa => pa.Product)
            .WithMany()
            .HasForeignKey(pa => pa.ProductId)
            .HasPrincipalKey(p => p.Id);
        
        modelBuilder.Entity<ProductAction>()
            .HasOne(pa => pa.Action)
            .WithMany()
            .HasForeignKey(pa => pa.ActionId)
            .HasPrincipalKey(a => a.Id);
        
        modelBuilder.Entity<CategoryAction>()
            .HasOne(ca => ca.Category)
            .WithMany()
            .HasForeignKey(ca => ca.CategoryId)
            .HasPrincipalKey(c => c.Id);
        
        modelBuilder.Entity<CategoryAction>()
            .HasOne(ca => ca.Action)
            .WithMany()
            .HasForeignKey(ca => ca.ActionId)
            .HasPrincipalKey(a => a.Id);
    }
}