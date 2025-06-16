using FoodDelivery.DAL.Entities;
using System;
using System.Threading.Tasks;

namespace FoodDelivery.DAL.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Restaurant> RestaurantRepository { get; }
        IRepository<MenuItem> MenuItemRepository { get; }
        IRepository<Category> CategoryRepository { get; }
        IRepository<Order> OrderRepository { get; }
        IRepository<OrderItem> OrderItemRepository { get; }

        Task<int> SaveAsync();
    }
}