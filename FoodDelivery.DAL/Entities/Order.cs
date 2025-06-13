using FoodDelivery.Common.Enums;
using System;
using System.Collections.Generic;

namespace FoodDelivery.DAL.Entities
{
    public class Order : BaseEntity
    {
        public string UserId { get; set; }
        public virtual User User { get; set; }
        public int RestaurantId { get; set; }
        public virtual Restaurant Restaurant { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public decimal TotalAmount { get; set; }
        public string DeliveryAddress { get; set; }
        public string ContactPhone { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }
}