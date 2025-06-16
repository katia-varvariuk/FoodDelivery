using FoodDelivery.BLL.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FoodDelivery.BLL.Interfaces
{
    public interface IRestaurantService
    {
        Task<PagedResultDto<RestaurantDto>> GetAllAsync(RestaurantFilterDto filterDto);
        Task<RestaurantDto> GetByIdAsync(int id);
        Task<RestaurantDto> CreateAsync(RestaurantDto restaurantDto);
        Task<RestaurantDto> UpdateAsync(int id, RestaurantDto restaurantDto);
        Task DeleteAsync(int id);
    }
}