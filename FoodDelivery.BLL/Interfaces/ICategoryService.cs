using FoodDelivery.BLL.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FoodDelivery.BLL.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetAllAsync();
        Task<CategoryDto> GetByIdAsync(int id);
        Task<CategoryDto> CreateAsync(CategoryDto categoryDto);
        Task<CategoryDto> UpdateAsync(int id, CategoryDto categoryDto);
        Task DeleteAsync(int id);
    }
}