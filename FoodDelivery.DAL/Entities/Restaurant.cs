using System.Collections.Generic;

namespace FoodDelivery.DAL.Entities
{
    public class Restaurant : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string LogoUrl { get; set; }
        public double Rating { get; set; }
        public bool IsActive { get; set; } = true;
        public virtual ICollection<MenuItem> MenuItems { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}