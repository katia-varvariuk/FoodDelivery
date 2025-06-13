namespace FoodDelivery.DAL.Entities
{
    public class MenuItem : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        public bool IsAvailable { get; set; } = true;
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }
        public int RestaurantId { get; set; }
        public virtual Restaurant Restaurant { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }
}