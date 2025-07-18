﻿namespace FoodDelivery.BLL.DTOs
{
    public class MenuItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        public bool IsAvailable { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int RestaurantId { get; set; }
        public string RestaurantName { get; set; }
    }
}