using FoodDelivery.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FoodDelivery.DAL.Configurations
{
    public class RestaurantConfiguration : IEntityTypeConfiguration<Restaurant>
    {
        public void Configure(EntityTypeBuilder<Restaurant> builder)
        {
            builder.HasKey(r => r.Id);

            builder.Property(r => r.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(r => r.Description)
                .HasMaxLength(500);

            builder.Property(r => r.Address)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(r => r.Phone)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(r => r.Email)
                .HasMaxLength(100);

            builder.Property(r => r.Rating)
                .HasDefaultValue(0);

            // Зв'язок з MenuItems
            builder.HasMany(r => r.MenuItems)
                .WithOne(m => m.Restaurant)
                .HasForeignKey(m => m.RestaurantId)
                .OnDelete(DeleteBehavior.Cascade);

            // Зв'язок з Orders
            builder.HasMany(r => r.Orders)
                .WithOne(o => o.Restaurant)
                .HasForeignKey(o => o.RestaurantId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}