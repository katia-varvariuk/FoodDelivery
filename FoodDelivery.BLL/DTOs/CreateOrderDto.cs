using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FoodDelivery.BLL.DTOs
{
    public class CreateOrderDto
    {
        [Required]
        public int RestaurantId { get; set; }

        [Required]
        public string DeliveryAddress { get; set; }

        [Required]
        [Phone]
        public string ContactPhone { get; set; }

        [Required]
        public List<OrderItemCreateDto> OrderItems { get; set; }
    }

    public class OrderItemCreateDto
    {
        [Required]
        public int MenuItemId { get; set; }

        [Required]
        [Range(1, 100)]
        public int Quantity { get; set; }
    }
}