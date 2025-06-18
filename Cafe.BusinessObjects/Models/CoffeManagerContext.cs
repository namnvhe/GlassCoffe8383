using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Cafe.BusinessObjects.Models;

public partial class CoffeManagerContext : DbContext
{
    public CoffeManagerContext()
    {
    }

    public CoffeManagerContext(DbContextOptions<CoffeManagerContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Bill> Bills { get; set; }

    public virtual DbSet<CoffeeTable> CoffeeTables { get; set; }

    public virtual DbSet<Discount> Discounts { get; set; }

    public virtual DbSet<DrinkRecipe> DrinkRecipes { get; set; }

    public virtual DbSet<DrinkType> DrinkTypes { get; set; }

    public virtual DbSet<Ingredient> Ingredients { get; set; }

    public virtual DbSet<MenuItem> MenuItems { get; set; }

    public virtual DbSet<MenuItemImage> MenuItemImages { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderItem> OrderItems { get; set; }

    public virtual DbSet<OrderItemTopping> OrderItemToppings { get; set; }

    public virtual DbSet<Size> Sizes { get; set; }

    public virtual DbSet<Topping> Toppings { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<VwMenuItemsWithImage> VwMenuItemsWithImages { get; set; }

    /*    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    #warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
            => optionsBuilder.UseSqlServer("server=(local); database=CoffeManager; uid=sa; pwd=123; Trusted_Connection=True; Encrypt=False");*/

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            IConfigurationRoot configuration = builder.Build();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Bill>(entity =>
        {
            entity.HasKey(e => e.BillId).HasName("PK__Bills__11F2FC4A8E7E7DCD");

            entity.ToTable(tb => tb.HasTrigger("trg_BillPaid_UpdateOrder"));

            entity.Property(e => e.BillId).HasColumnName("BillID");
            entity.Property(e => e.GeneratedTime).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.PaymentStatus)
                .HasMaxLength(20)
                .HasDefaultValue("Unpaid");
            entity.Property(e => e.Qrcode)
                .HasMaxLength(100)
                .HasColumnName("QRCode");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(12, 2)");

            entity.HasOne(d => d.Order).WithMany(p => p.Bills)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Bills__OrderID__4F7CD00D");
        });

        modelBuilder.Entity<CoffeeTable>(entity =>
        {
            entity.HasKey(e => e.TableId).HasName("PK__CoffeeTa__7D5F018E57DF2074");

            entity.HasIndex(e => e.Qrcode, "UQ__CoffeeTa__5B869AD977FD0792").IsUnique();

            entity.HasIndex(e => e.TableNumber, "UQ__CoffeeTa__E8E0DB52052E4169").IsUnique();

            entity.Property(e => e.TableId).HasColumnName("TableID");
            entity.Property(e => e.Qrcode)
                .HasMaxLength(100)
                .HasColumnName("QRCode");
        });

        modelBuilder.Entity<Discount>(entity =>
        {
            entity.HasKey(e => e.DiscountId).HasName("PK__Discount__E43F6DF67B2CC5CE");

            entity.HasIndex(e => e.Code, "UQ__Discount__A25C5AA797A58070").IsUnique();

            entity.Property(e => e.DiscountId).HasColumnName("DiscountID");
            entity.Property(e => e.Code).HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.DiscountType).HasMaxLength(10);
            entity.Property(e => e.ExpiryDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Value).HasColumnType("decimal(10, 2)");
        });

        modelBuilder.Entity<DrinkRecipe>(entity =>
        {
            entity.HasKey(e => e.RecipeId).HasName("PK__DrinkRec__FDD988D0D23D462A");

            entity.Property(e => e.RecipeId).HasColumnName("RecipeID");
            entity.Property(e => e.IngredientId).HasColumnName("IngredientID");
            entity.Property(e => e.MenuItemId).HasColumnName("MenuItemID");

            entity.HasOne(d => d.Ingredient).WithMany(p => p.DrinkRecipes)
                .HasForeignKey(d => d.IngredientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DrinkReci__Ingre__628FA481");

            entity.HasOne(d => d.MenuItem).WithMany(p => p.DrinkRecipes)
                .HasForeignKey(d => d.MenuItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DrinkReci__MenuI__619B8048");
        });

        modelBuilder.Entity<DrinkType>(entity =>
        {
            entity.HasKey(e => e.DrinkTypeId).HasName("PK__DrinkTyp__F6D6B7458AFFA2EB");

            entity.HasIndex(e => e.TypeName, "UQ__DrinkTyp__D4E7DFA8AEE935B3").IsUnique();

            entity.Property(e => e.DrinkTypeId).HasColumnName("DrinkTypeID");
            entity.Property(e => e.TypeName).HasMaxLength(50);
        });

        modelBuilder.Entity<Ingredient>(entity =>
        {
            entity.HasKey(e => e.IngredientId).HasName("PK__Ingredie__BEAEB27AFE894EE4");

            entity.Property(e => e.IngredientId).HasColumnName("IngredientID");
            entity.Property(e => e.IngredientImage).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(10, 2)");
        });

        modelBuilder.Entity<MenuItem>(entity =>
        {
            entity.HasKey(e => e.MenuItemId).HasName("PK__MenuItem__8943F702E5F00177");

            entity.Property(e => e.MenuItemId).HasColumnName("MenuItemID");
            entity.Property(e => e.DrinkTypeId).HasColumnName("DrinkTypeID");
            entity.Property(e => e.IsAvailable).HasDefaultValue(true);
            entity.Property(e => e.MenuItemImage).HasMaxLength(255);
            entity.Property(e => e.MinStockLevel).HasDefaultValue(5);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.DrinkType).WithMany(p => p.MenuItems)
                .HasForeignKey(d => d.DrinkTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MenuItems_DrinkTypes");

            entity.HasMany(d => d.Toppings).WithMany(p => p.MenuItems)
                .UsingEntity<Dictionary<string, object>>(
                    "MenuItemTopping",
                    r => r.HasOne<Topping>().WithMany()
                        .HasForeignKey("ToppingId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__MenuItemT__Toppi__693CA210"),
                    l => l.HasOne<MenuItem>().WithMany()
                        .HasForeignKey("MenuItemId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__MenuItemT__MenuI__68487DD7"),
                    j =>
                    {
                        j.HasKey("MenuItemId", "ToppingId");
                        j.ToTable("MenuItemToppings");
                        j.IndexerProperty<int>("MenuItemId").HasColumnName("MenuItemID");
                        j.IndexerProperty<int>("ToppingId").HasColumnName("ToppingID");
                    });
        });

        modelBuilder.Entity<MenuItemImage>(entity =>
        {
            entity.HasKey(e => e.ImageId);

            entity.HasIndex(e => e.IsMainImage, "IX_MenuItemImages_IsMainImage");

            entity.HasIndex(e => e.MenuItemId, "IX_MenuItemImages_MenuItemID");

            entity.Property(e => e.ImageId).HasColumnName("ImageID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ImageName).HasMaxLength(255);
            entity.Property(e => e.ImageUrl).HasMaxLength(500);
            entity.Property(e => e.MenuItemId).HasColumnName("MenuItemID");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.MenuItem).WithMany(p => p.MenuItemImages)
                .HasForeignKey(d => d.MenuItemId)
                .HasConstraintName("FK_MenuItemImages_MenuItems");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__Orders__C3905BAF7E354220");

            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.DiscountId).HasColumnName("DiscountID");
            entity.Property(e => e.OrderTime).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Pending");
            entity.Property(e => e.TableId).HasColumnName("TableID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Discount).WithMany(p => p.Orders)
                .HasForeignKey(d => d.DiscountId)
                .HasConstraintName("FK_Orders_Discounts");

            entity.HasOne(d => d.Table).WithMany(p => p.Orders)
                .HasForeignKey(d => d.TableId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Orders__TableID__4316F928");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Orders__UserID__440B1D61");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.OrderItemId).HasName("PK__OrderIte__57ED06A1BC9EF6CC");

            entity.ToTable(tb => tb.HasTrigger("tr_UpdateStock_OrderItems"));

            entity.Property(e => e.OrderItemId).HasColumnName("OrderItemID");
            entity.Property(e => e.DiscountId).HasColumnName("DiscountID");
            entity.Property(e => e.MenuItemId).HasColumnName("MenuItemID");
            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.SizeId).HasColumnName("SizeID");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Pending");
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Discount).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.DiscountId)
                .HasConstraintName("FK_OrderItems_Discounts");

            entity.HasOne(d => d.MenuItem).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.MenuItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderItem__MenuI__4AB81AF0");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderItem__Order__49C3F6B7");

            entity.HasOne(d => d.Size).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.SizeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderItems_Sizes");
        });

        modelBuilder.Entity<OrderItemTopping>(entity =>
        {
            entity.HasKey(e => e.OrderItemToppingId).HasName("PK__OrderIte__57ACACE41D05B977");

            entity.ToTable(tb => tb.HasTrigger("tr_UpdateStock_OrderItemToppings"));

            entity.Property(e => e.OrderItemToppingId).HasColumnName("OrderItemToppingID");
            entity.Property(e => e.OrderItemId).HasColumnName("OrderItemID");
            entity.Property(e => e.Quantity).HasDefaultValue(1);
            entity.Property(e => e.ToppingId).HasColumnName("ToppingID");

            entity.HasOne(d => d.OrderItem).WithMany(p => p.OrderItemToppings)
                .HasForeignKey(d => d.OrderItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderItem__Order__6C190EBB");

            entity.HasOne(d => d.Topping).WithMany(p => p.OrderItemToppings)
                .HasForeignKey(d => d.ToppingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderItem__Toppi__6D0D32F4");
        });

        modelBuilder.Entity<Size>(entity =>
        {
            entity.HasKey(e => e.SizeId).HasName("PK__Sizes__83BD095AE3E20585");

            entity.HasIndex(e => e.Name, "UQ__Sizes__737584F69EC823DB").IsUnique();

            entity.Property(e => e.SizeId).HasColumnName("SizeID");
            entity.Property(e => e.ExtraPrice).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Name).HasMaxLength(20);
        });

        modelBuilder.Entity<Topping>(entity =>
        {
            entity.HasKey(e => e.ToppingId).HasName("PK__Toppings__EE02CCE52F00DC73");

            entity.HasIndex(e => e.Name, "UQ__Toppings__737584F66F52EEBA").IsUnique();

            entity.Property(e => e.ToppingId).HasColumnName("ToppingID");
            entity.Property(e => e.IsAvailable).HasDefaultValue(true);
            entity.Property(e => e.MinStockLevel).HasDefaultValue(10);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.ToppingImage).HasMaxLength(255);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCAC59F42CBC");

            entity.HasIndex(e => e.Email, "IX_Users_Email");

            entity.HasIndex(e => e.RefreshToken, "IX_Users_RefreshToken");

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.EmailVerificationToken).HasMaxLength(255);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.PasswordResetTokenExpiry).HasColumnType("datetime");
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.RefreshToken).HasMaxLength(500);
            entity.Property(e => e.Role).HasMaxLength(20);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<VwMenuItemsWithImage>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_MenuItemsWithImages");

            entity.Property(e => e.DrinkType).HasMaxLength(50);
            entity.Property(e => e.MainImageUrl).HasMaxLength(500);
            entity.Property(e => e.MenuItemId).HasColumnName("MenuItemID");
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
