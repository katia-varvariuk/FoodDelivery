using System.Collections.Generic;

namespace FoodDelivery.DAL.Entities
{
    public class Category : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public virtual ICollection<MenuItem> MenuItems { get; set; }
    }
}