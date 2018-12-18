using Microsoft.EntityFrameworkCore;
using PizzaStoreData.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PizzaStoreData.DataAccess
{
    public class PizzaStoreDBContext : DbContext
    {
        public PizzaStoreDBContext()
        {
        }

        // Call base constructor with the correct options 
        //  in order to have this class connect to the 
        //  DB through the connectionstring used to construct
        //  the options provided.
        public PizzaStoreDBContext(DbContextOptions<PizzaStoreDBContext> options)
            : base(options) { }

        #region Standard Tables
        // Table of all possible ingredients a location can
        //  have stocked with their respective price
        public virtual DbSet<Ingredient> Ingredient { get; set; }
        // Table of all location names
        public virtual DbSet<Location> Location { get; set; }
        // Table of all orders with what location it was placed
        //  at, what user placed it, time placed, and total
        //  cost of the order
        public virtual DbSet<Order> Order { get; set; }
        // Table of all users with first and last names, 
        //  and potential default location to order from
        public virtual DbSet<User> User { get; set; }
        #endregion

        #region Junction Tables
        // Can have any number of locations which each have
        //  any number of ingredients
        public virtual DbSet<InventoryJunction> InventoryJunction { get; set; }
        // Can have any number of orders which each have any
        //  number of pizzas
        public virtual DbSet<OrderJunction> OrderJunction { get; set; }
        // Can have any number of pizzas which each can have
        //  any number of ingredients
        public virtual DbSet<PizzaJunction> PizzaJunction { get; set; }
        #endregion


        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    if (!optionsBuilder.IsConfigured)
        //    {
        //    }
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.0-rtm-35687");

            modelBuilder.Entity<Ingredient>(entity =>
            {
                entity.ToTable("Ingredient", "MVCPizzaStore");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .IsRequired();

                entity.HasIndex(e => e.Name)
                    .IsUnique();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(128);
            });

            modelBuilder.Entity<Location>(entity =>
            {
                entity.ToTable("Location", "MVCPizzaStore");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .IsRequired();

                entity.HasIndex(e => e.Name)
                    .IsUnique();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(128);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User", "MVCPizzaStore");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .IsRequired();

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(128);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(128);
                
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("Order", "MVCPizzaStore");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .IsRequired();

                entity.Property(e => e.LocationId)
                    .ValueGeneratedNever()
                    .IsRequired();

                entity.Property(e => e.UserId)
                    .ValueGeneratedNever()
                    .IsRequired();

                entity.HasOne(e => e.Location)
                    .WithMany(p => p.Order)
                    .HasForeignKey(d => d.LocationId);

                entity.HasOne(e => e.User)
                    .WithMany(p => p.Order)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<InventoryJunction>(entity =>
            {
                entity.HasKey(e => new { e.LocationId, e.IngredientId });

                entity.ToTable("InventoryJunction", "MVCPizzaStore");

                entity.Property(e => e.LocationId)
                    .ValueGeneratedNever()
                    .IsRequired();

                entity.Property(e => e.IngredientId)
                    .ValueGeneratedNever()
                    .IsRequired();

                entity.HasOne(d => d.Ingredient)
                    .WithMany(i => i.InventoryJunction)
                    .HasForeignKey(d => d.IngredientId)
                    .OnDelete(DeleteBehavior.ClientSetNull);    

                entity.HasOne(d => d.Location)
                    .WithMany(l => l.InventoryJunction)
                    .HasForeignKey(d => d.LocationId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<PizzaJunction>(entity =>
            {
                entity.HasKey(e => new { e.PizzaId, e.IngredientId });

                entity.ToTable("PizzaJunction", "MVCPizzaStore");

                entity.Property(e => e.PizzaId)
                    .ValueGeneratedNever()
                    .IsRequired();

                entity.Property(e => e.IngredientId)
                    .ValueGeneratedNever()
                    .IsRequired();

                entity.HasOne(d => d.Ingredient)
                    .WithMany(i => i.PizzaJunction)
                    .HasForeignKey(d => d.IngredientId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

            });

            modelBuilder.Entity<OrderJunction>(entity =>
            {
                entity.HasKey(e => new { e.OrderId, e.PizzaId });

                entity.ToTable("OrderJunction", "MVCPizzaStore");

                entity.Property(e => e.OrderId)
                    .ValueGeneratedNever()
                    .IsRequired();

                entity.Property(e => e.PizzaId)
                    .ValueGeneratedNever()
                    .IsRequired();

                //entity.HasOne(d => d.Order)
                //    .WithMany(i => i.OrderJunction)
                //    .HasForeignKey(d => d.OrderId)
                //    .OnDelete(DeleteBehavior.ClientSetNull);

                //entity.HasOne(d => d.PizzaJunction)
                //    .WithMany(i => i.OrderJunction)
                //    .HasForeignKey(d => new { d.OrderId, d.PizzaId })
                //    .OnDelete(DeleteBehavior.ClientSetNull);
            });
        }
    }
}
