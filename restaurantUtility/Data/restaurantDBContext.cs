using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using restaurantUtility.Models;

#nullable disable

namespace restaurantUtility.Data
{
    public partial class restaurantDBContext : DbContext
    {
        public restaurantDBContext()
        {
        }

        public restaurantDBContext(DbContextOptions<restaurantDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Cart> Carts { get; set; }
        public virtual DbSet<Favourite> Favourites { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderDetail> OrderDetails { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Store> Stores { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySQL("name=restaurantDB");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cart>(entity =>
            {
                entity.HasKey(e => new { e.UserName, e.ProductId })
                    .HasName("PRIMARY");

                entity.Property(e => e.Instruction).HasDefaultValueSql("'NULL'");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.Carts)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("cart_ibfk_2");

                entity.HasOne(d => d.UserNameNavigation)
                    .WithMany(p => p.Carts)
                    .HasForeignKey(d => d.UserName)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("cart_ibfk_1");
            });

            modelBuilder.Entity<Favourite>(entity =>
            {
                entity.HasKey(e => new { e.UserName, e.ProductId })
                    .HasName("PRIMARY");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.Favourites)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("favourite_ibfk_2");

                entity.HasOne(d => d.UserNameNavigation)
                    .WithMany(p => p.Favourites)
                    .HasForeignKey(d => d.UserName)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("favourite_ibfk_1");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasOne(d => d.Store)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.StoreId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("order_storeId_constraint");

                entity.HasOne(d => d.UserNameNavigation)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.UserName)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("order_user_constraint");
            });

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.HasKey(e => new { e.OrderId, e.ProductId })
                    .HasName("PRIMARY");

                entity.Property(e => e.Instruction).HasDefaultValueSql("'NULL'");

                entity.Property(e => e.Rating).HasDefaultValueSql("'NULL'");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("order_details_ibfk_1");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("order_details_ibfk_2");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasOne(d => d.Store)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.StoreId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("product_store_constraint");
            });

            modelBuilder.Entity<Store>(entity =>
            {
                entity.Property(e => e.ImgLoc).HasDefaultValueSql("'NULL'");

                entity.Property(e => e.Rating).HasDefaultValueSql("'NULL'");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserName)
                    .HasName("PRIMARY");

                entity.Property(e => e.Email).HasDefaultValueSql("'NULL'");

                entity.Property(e => e.FirstName).HasDefaultValueSql("'NULL'");

                entity.Property(e => e.LastName).HasDefaultValueSql("'NULL'");

                entity.Property(e => e.PhoneNumber).HasDefaultValueSql("'NULL'");

                entity.Property(e => e.StoreId).HasDefaultValueSql("'NULL'");
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasKey(e => new { e.UserName, e.Role })
                    .HasName("PRIMARY");

                entity.HasOne(d => d.RoleNavigation)
                    .WithMany(p => p.UserRoles)
                    .HasForeignKey(d => d.Role)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("user_role_ibfk_1");

                entity.HasOne(d => d.UserNameNavigation)
                    .WithMany(p => p.UserRoles)
                    .HasForeignKey(d => d.UserName)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("user_role_ibfk_2");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
