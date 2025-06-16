namespace FoodDelivery.BLL.DTOs
{
    public class RestaurantDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string LogoUrl { get; set; }
        public double Rating { get; set; }
        public bool IsActive { get; set; }
    }
}