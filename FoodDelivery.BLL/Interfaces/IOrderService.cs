using FoodDelivery.BLL.DTOs;
using FoodDelivery.Common.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FoodDelivery.BLL.Interfaces
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderDto>> GetAllByUserIdAsync(string userId);
        Task<IEnumerable<OrderDto>> GetAllByRestaurantIdAsync(int restaurantId);
        Task<OrderDto> GetByIdAsync(int id);
        Task<OrderDto> CreateAsync(string userId, CreateOrderDto createOrderDto);
        Task<OrderDto> UpdateStatusAsync(int id, OrderStatus status);
        Task DeleteAsync(int id);
    }
}