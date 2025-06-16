using FoodDelivery.BLL.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FoodDelivery.BLL.Interfaces
{
    public interface IMenuItemService
    {
        Task<IEnumerable<MenuItemDto>> GetAllByRestaurantIdAsync(int restaurantId);
        Task<MenuItemDto> GetByIdAsync(int id);
        Task<MenuItemDto> CreateAsync(MenuItemDto menuItemDto);
        Task<MenuItemDto> UpdateAsync(int id, MenuItemDto menuItemDto);
        Task DeleteAsync(int id);
    }
}