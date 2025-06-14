using FoodDelivery.Common.Interfaces;
using FoodDelivery.DAL.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;

namespace FoodDelivery.DAL.Data
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Restaurant> Restaurants { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Застосовуємо всі конфігурації
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            // Глобальний конвертер для DateTime
            var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
                v => v.Kind == DateTimeKind.Utc ? v : DateTime.SpecifyKind(v, DateTimeKind.Utc),
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

            var nullableDateTimeConverter = new ValueConverter<DateTime?, DateTime?>(
                v => v.HasValue ? (v.Value.Kind == DateTimeKind.Utc ? v : DateTime.SpecifyKind(v.Value, DateTimeKind.Utc)) : null,
                v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : null);

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(DateTime))
                    {
                        property.SetValueConverter(dateTimeConverter);
                    }
                    else if (property.ClrType == typeof(DateTime?))
                    {
                        property.SetValueConverter(nullableDateTimeConverter);
                    }
                }
            }
        }

        public override int SaveChanges()
        {
            ProcessDateTimeProperties();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            ProcessDateTimeProperties();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private void ProcessDateTimeProperties()
        {
            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
                {
                    foreach (var property in entry.Properties)
                    {
                        if (property.Metadata.ClrType == typeof(DateTime) && property.CurrentValue != null)
                        {
                            var dt = (DateTime)property.CurrentValue;
                            if (dt.Kind != DateTimeKind.Utc)
                            {
                                property.CurrentValue = DateTime.SpecifyKind(dt, DateTimeKind.Utc);
                            }
                        }
                        else if (property.Metadata.ClrType == typeof(DateTime?) && property.CurrentValue != null)
                        {
                            var dt = (DateTime?)property.CurrentValue;
                            if (dt.HasValue && dt.Value.Kind != DateTimeKind.Utc)
                            {
                                property.CurrentValue = DateTime.SpecifyKind(dt.Value, DateTimeKind.Utc);
                            }
                        }
                    }
                }
            }
        }
    }
}