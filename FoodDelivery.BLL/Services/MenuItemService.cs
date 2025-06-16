using AutoMapper;
using FoodDelivery.BLL.DTOs;
using FoodDelivery.BLL.Interfaces;
using FoodDelivery.DAL.Entities;
using FoodDelivery.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FoodDelivery.BLL.Services
{
    public class MenuItemService : IMenuItemService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public MenuItemService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<MenuItemDto>> GetAllByRestaurantIdAsync(int restaurantId)
        {
            var menuItems = await _unitOfWork.MenuItemRepository.GetAsync(
                m => m.RestaurantId == restaurantId && m.IsAvailable,
                null,
                "Category,Restaurant"
            );

            return _mapper.Map<IEnumerable<MenuItemDto>>(menuItems);
        }

        public async Task<MenuItemDto> GetByIdAsync(int id)
        {
            var menuItem = await _unitOfWork.MenuItemRepository.FirstOrDefaultAsync(m => m.Id == id);

            if (menuItem == null)
            {
                return null;
            }

            // Завантажуємо зв'язані сутності
            await _unitOfWork.MenuItemRepository.GetAsync(
                m => m.Id == id,
                null,
                "Category,Restaurant"
            );

            return _mapper.Map<MenuItemDto>(menuItem);
        }

        public async Task<MenuItemDto> CreateAsync(MenuItemDto menuItemDto)
        {
            var menuItem = _mapper.Map<MenuItem>(menuItemDto);

            await _unitOfWork.MenuItemRepository.AddAsync(menuItem);
            await _unitOfWork.SaveAsync();

            // Завантажуємо зв'язані сутності для коректного мапінгу
            await _unitOfWork.MenuItemRepository.GetAsync(
                m => m.Id == menuItem.Id,
                null,
                "Category,Restaurant"
            );

            return _mapper.Map<MenuItemDto>(menuItem);
        }

        public async Task<MenuItemDto> UpdateAsync(int id, MenuItemDto menuItemDto)
        {
            var menuItem = await _unitOfWork.MenuItemRepository.GetByIdAsync(id);

            if (menuItem == null)
            {
                return null;
            }

            _mapper.Map(menuItemDto, menuItem);
            menuItem.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.MenuItemRepository.Update(menuItem);
            await _unitOfWork.SaveAsync();

            // Завантажуємо зв'язані сутності для коректного мапінгу
            await _unitOfWork.MenuItemRepository.GetAsync(
                m => m.Id == menuItem.Id,
                null,
                "Category,Restaurant"
            );

            return _mapper.Map<MenuItemDto>(menuItem);
        }

        public async Task DeleteAsync(int id)
        {
            await _unitOfWork.MenuItemRepository.RemoveAsync(id);
            await _unitOfWork.SaveAsync();
        }
    }
}