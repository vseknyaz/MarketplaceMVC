using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using MarketplaceDomain.Model;

namespace MarketplaceInfrastructure
{
    public partial class MarketplaceContext : DbContext
    {
        public MarketplaceContext()
        {
        }

        public MarketplaceContext(DbContextOptions<MarketplaceContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Client> Clients { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderProduct> OrderProducts { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Shop> Shops { get; set; }
        public virtual DbSet<Statusess> Statusesses { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseSqlServer(
                "Server=DESKTOP-SQPUCEH\\SQLEXPRESS; Database=Marketplace; Trusted_Connection=True; TrustServerCertificate=True; ");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.CategoryName);
                entity.Property(e => e.CategoryName)
                    .HasMaxLength(50)
                    .IsFixedLength();
            });

            modelBuilder.Entity<Client>(entity =>
            {
                entity.HasKey(e => e.ClientId)
                      .HasName("PK__Customer__A4AE64B8FB8581E9");

                entity.HasIndex(e => e.PhoneNumber, "UQ__Customer__85FB4E389B62B1D0").IsUnique();
                entity.HasIndex(e => e.Email, "UQ__Customer__A9D105345935C0CA").IsUnique();

                entity.Property(e => e.ClientId).HasColumnName("ClientID");
                entity.Property(e => e.Email).HasMaxLength(255);
                entity.Property(e => e.FullName).HasMaxLength(255);
                entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.OrderId)
                      .HasName("PK__Orders__C3905BAF86F3CDED");

                entity.Property(e => e.OrderId).HasColumnName("OrderID");

                // При видаленні клієнта - видаляємо всі його замовлення (Cascade)
                entity.HasOne(d => d.Client)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.ClientId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Orders_Clients");

                // При видаленні статусу - встановлюємо NULL (SetNull)
                entity.HasOne(d => d.Status)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_Orders_Statuses");
            });

            modelBuilder.Entity<OrderProduct>(entity =>
            {
                entity.HasKey(e => e.OrderProductId)
                      .HasName("PK__OrderPro__E6A13246FBE43449");

                entity.Property(e => e.OrderProductId).HasColumnName("OrderProduct_ID");
                entity.Property(e => e.AdditionalInfo).HasMaxLength(200);
                entity.Property(e => e.OrderId).HasColumnName("Order_ID");
                entity.Property(e => e.ProductId).HasColumnName("Product_ID");

                // При видаленні замовлення - видаляємо всі позиції (Cascade)
                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderProducts)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_OrderProducts_Orders");

                // При видаленні товару - забороняємо (Restrict)
                entity.HasOne(d => d.Product)
                    .WithMany(p => p.OrderProducts)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_OrderProducts_Products");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.ProductId)
                      .HasName("PK__Products__B40CC6ED8DE87286");

                entity.Property(e => e.ProductId).HasColumnName("ProductID");
                entity.Property(e => e.CategoryId)
                    .HasMaxLength(50)
                    .IsFixedLength();
                entity.Property(e => e.Price).HasColumnType("money");
                entity.Property(e => e.ProductName).HasMaxLength(255);

                // При видаленні категорії - забороняємо (Restrict)
                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Products_Categories");

                // При видаленні магазину - забороняємо (Restrict)
                entity.HasOne(d => d.Shop)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.ShopId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Products_Shops");
            });

            modelBuilder.Entity<Shop>(entity =>
            {
                entity.HasKey(e => e.ShopId)
                      .HasName("PK__Sellers__7FE3DBA1168EE135");

                entity.Property(e => e.ShopId).HasColumnName("ShopID");
                entity.Property(e => e.ShopName).HasMaxLength(255);
            });

            modelBuilder.Entity<Statusess>(entity =>
            {
                entity.HasKey(e => e.StatusId).HasName("PK_Statusess");
                entity.ToTable("Statusess");

                entity.Property(e => e.StatusId).HasColumnName("StatusID");
                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsFixedLength();
                entity.Property(e => e.Description)
                    .HasMaxLength(50)
                    .IsFixedLength();
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
