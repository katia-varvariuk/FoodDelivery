using AutoMapper;
using FoodDelivery.BLL.DTOs;
using FoodDelivery.BLL.Interfaces;
using FoodDelivery.DAL.Entities;
using FoodDelivery.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FoodDelivery.BLL.Services
{
    public class RestaurantService : IRestaurantService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public RestaurantService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResultDto<RestaurantDto>> GetAllAsync(RestaurantFilterDto filterDto)
        {
            // Створюємо фільтр
            Expression<Func<Restaurant, bool>> filter = r => r.IsActive;

            if (!string.IsNullOrEmpty(filterDto.SearchTerm))
            {
                var searchTerm = filterDto.SearchTerm.ToLower();
                filter = filter.And(r => r.Name.ToLower().Contains(searchTerm) ||
                                        r.Description.ToLower().Contains(searchTerm) ||
                                        r.Address.ToLower().Contains(searchTerm));
            }

            if (filterDto.MinRating.HasValue)
            {
                filter = filter.And(r => r.Rating >= filterDto.MinRating.Value);
            }

            if (filterDto.MaxRating.HasValue)
            {
                filter = filter.And(r => r.Rating <= filterDto.MaxRating.Value);
            }

            // Створюємо сортування
            Func<IQueryable<Restaurant>, IOrderedQueryable<Restaurant>> orderBy = null;

            switch (filterDto.SortBy.ToLower())
            {
                case "name":
                    orderBy = filterDto.SortDesc
                        ? q => q.OrderByDescending(r => r.Name)
                        : q => q.OrderBy(r => r.Name);
                    break;
                case "rating":
                    orderBy = filterDto.SortDesc
                        ? q => q.OrderByDescending(r => r.Rating)
                        : q => q.OrderBy(r => r.Rating);
                    break;
                default:
                    orderBy = q => q.OrderBy(r => r.Name);
                    break;
            }

            // Отримуємо загальну кількість
            var totalCount = await _unitOfWork.RestaurantRepository.CountAsync(filter);

            // Отримуємо ресторани з пагінацією
            var restaurants = await _unitOfWork.RestaurantRepository.GetAsync(
                filter,
                orderBy,
                ""
            );

            var pagedRestaurants = restaurants
                .Skip((filterDto.PageNumber - 1) * filterDto.PageSize)
                .Take(filterDto.PageSize)
                .ToList();

            // Мапимо результат
            var restaurantDtos = _mapper.Map<List<RestaurantDto>>(pagedRestaurants);

            return new PagedResultDto<RestaurantDto>
            {
                Items = restaurantDtos,
                TotalCount = totalCount,
                PageNumber = filterDto.PageNumber,
                PageSize = filterDto.PageSize
            };
        }

        public async Task<RestaurantDto> GetByIdAsync(int id)
        {
            var restaurant = await _unitOfWork.RestaurantRepository.GetByIdAsync(id);

            if (restaurant == null)
            {
                return null;
            }

            return _mapper.Map<RestaurantDto>(restaurant);
        }

        public async Task<RestaurantDto> CreateAsync(RestaurantDto restaurantDto)
        {
            var restaurant = _mapper.Map<Restaurant>(restaurantDto);
            restaurant.CreatedAt = DateTime.UtcNow;

            await _unitOfWork.RestaurantRepository.AddAsync(restaurant);
            await _unitOfWork.SaveAsync();

            return _mapper.Map<RestaurantDto>(restaurant);
        }

        public async Task<RestaurantDto> UpdateAsync(int id, RestaurantDto restaurantDto)
        {
            var restaurant = await _unitOfWork.RestaurantRepository.GetByIdAsync(id);

            if (restaurant == null)
            {
                return null;
            }

            _mapper.Map(restaurantDto, restaurant);
            restaurant.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.RestaurantRepository.Update(restaurant);
            await _unitOfWork.SaveAsync();

            return _mapper.Map<RestaurantDto>(restaurant);
        }

        public async Task DeleteAsync(int id)
        {
            await _unitOfWork.RestaurantRepository.RemoveAsync(id);
            await _unitOfWork.SaveAsync();
        }
    }

    // Розширення для комбінування предикатів
    public static class ExpressionExtensions
    {
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
        {
            var param = Expression.Parameter(typeof(T), "x");
            var body = Expression.AndAlso(
                Expression.Invoke(left, param),
                Expression.Invoke(right, param)
            );
            return Expression.Lambda<Func<T, bool>>(body, param);
        }
    }
}