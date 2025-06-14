using FoodDelivery.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FoodDelivery.DAL.Configurations
{
    public class MenuItemConfiguration : IEntityTypeConfiguration<MenuItem>
    {
        public void Configure(EntityTypeBuilder<MenuItem> builder)
        {
            builder.HasKey(m => m.Id);

            builder.Property(m => m.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(m => m.Description)
                .HasMaxLength(500);

            builder.Property(m => m.Price)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            // Зв'язок з Category
            builder.HasOne(m => m.Category)
                .WithMany(c => c.MenuItems)
                .HasForeignKey(m => m.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Зв'язок з Restaurant
            builder.HasOne(m => m.Restaurant)
                .WithMany(r => r.MenuItems)
                .HasForeignKey(m => m.RestaurantId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}