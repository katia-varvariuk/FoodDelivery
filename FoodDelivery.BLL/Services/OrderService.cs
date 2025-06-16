using AutoMapper;
using FoodDelivery.BLL.DTOs;
using FoodDelivery.BLL.Interfaces;
using FoodDelivery.Common.Enums;
using FoodDelivery.DAL.Entities;
using FoodDelivery.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FoodDelivery.BLL.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public OrderService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<OrderDto>> GetAllByUserIdAsync(string userId)
        {
            var orders = await _unitOfWork.OrderRepository.GetAsync(
                o => o.UserId == userId,
                o => o.OrderByDescending(o => o.CreatedAt),
                "Restaurant,OrderItems,OrderItems.MenuItem"
            );

            return _mapper.Map<IEnumerable<OrderDto>>(orders);
        }

        public async Task<IEnumerable<OrderDto>> GetAllByRestaurantIdAsync(int restaurantId)
        {
            var orders = await _unitOfWork.OrderRepository.GetAsync(
                o => o.RestaurantId == restaurantId,
                o => o.OrderByDescending(o => o.CreatedAt),
                "User,OrderItems,OrderItems.MenuItem"
            );

            return _mapper.Map<IEnumerable<OrderDto>>(orders);
        }

        public async Task<OrderDto> GetByIdAsync(int id)
        {
            var order = await _unitOfWork.OrderRepository.GetAsync(
                o => o.Id == id,
                null,
                "User,Restaurant,OrderItems,OrderItems.MenuItem"
            );

            var orderEntity = order.FirstOrDefault();
            if (orderEntity == null)
            {
                return null;
            }

            return _mapper.Map<OrderDto>(orderEntity);
        }

        public async Task<OrderDto> CreateAsync(string userId, CreateOrderDto createOrderDto)
        {
            // Перевіряємо, чи існує ресторан
            var restaurant = await _unitOfWork.RestaurantRepository.GetByIdAsync(createOrderDto.RestaurantId);

            if (restaurant == null)
            {
                throw new Exception("Ресторан не знайдено");
            }

            // Отримуємо всі страви для перевірки та розрахунку вартості
            var menuItemIds = createOrderDto.OrderItems.Select(oi => oi.MenuItemId).ToList();
            var menuItems = await _unitOfWork.MenuItemRepository.GetAsync(
                mi => menuItemIds.Contains(mi.Id) && mi.IsAvailable,
                null,
                ""
            );

            // Перевіряємо, чи всі страви існують та доступні
            if (menuItems.Count() != menuItemIds.Count)
            {
                throw new Exception("Деякі страви не існують або недоступні");
            }

            // Перевіряємо, чи всі страви належать до одного ресторану
            if (menuItems.Any(mi => mi.RestaurantId != createOrderDto.RestaurantId))
            {
                throw new Exception("Всі страви повинні належати до одного ресторану");
            }

            // Створюємо замовлення
            var order = new Order
            {
                UserId = userId,
                RestaurantId = createOrderDto.RestaurantId,
                DeliveryAddress = createOrderDto.DeliveryAddress,
                ContactPhone = createOrderDto.ContactPhone,
                Status = OrderStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                OrderItems = new List<OrderItem>()
            };

            // Додаємо страви до замовлення
            decimal totalAmount = 0;

            foreach (var orderItemDto in createOrderDto.OrderItems)
            {
                var menuItem = menuItems.First(mi => mi.Id == orderItemDto.MenuItemId);

                var orderItem = new OrderItem
                {
                    MenuItemId = orderItemDto.MenuItemId,
                    Quantity = orderItemDto.Quantity,
                    Price = menuItem.Price,
                    CreatedAt = DateTime.UtcNow
                };

                totalAmount += orderItem.Price * orderItem.Quantity;
                order.OrderItems.Add(orderItem);
            }

            order.TotalAmount = totalAmount;

            // Зберігаємо замовлення
            await _unitOfWork.OrderRepository.AddAsync(order);
            await _unitOfWork.SaveAsync();

            // Завантажуємо зв'язані сутності для коректного мапінгу
            var createdOrder = await _unitOfWork.OrderRepository.GetAsync(
                o => o.Id == order.Id,
                null,
                "User,Restaurant,OrderItems,OrderItems.MenuItem"
            );

            return _mapper.Map<OrderDto>(createdOrder.First());
        }

        public async Task<OrderDto> UpdateStatusAsync(int id, OrderStatus status)
        {
            var order = await _unitOfWork.OrderRepository.GetByIdAsync(id);

            if (order == null)
            {
                return null;
            }

            order.Status = status;
            order.UpdatedAt = DateTime.UtcNow;

            // Якщо статус "Доставлено", то оновлюємо дату доставки
            if (status == OrderStatus.Delivered)
            {
                order.DeliveredAt = DateTime.UtcNow;
            }

            _unitOfWork.OrderRepository.Update(order);
            await _unitOfWork.SaveAsync();

            // Завантажуємо зв'язані сутності для коректного мапінгу
            var updatedOrder = await _unitOfWork.OrderRepository.GetAsync(
                o => o.Id == order.Id,
                null,
                "User,Restaurant,OrderItems,OrderItems.MenuItem"
            );

            return _mapper.Map<OrderDto>(updatedOrder.First());
        }

        public async Task DeleteAsync(int id)
        {
            var order = await _unitOfWork.OrderRepository.GetByIdAsync(id);

            if (order == null)
            {
                throw new Exception("Замовлення не знайдено");
            }

            // Перевіряємо, чи можна видалити замовлення (тільки в статусі Pending або Cancelled)
            if (order.Status != OrderStatus.Pending && order.Status != OrderStatus.Cancelled)
            {
                throw new Exception("Можна видаляти тільки замовлення в статусі 'Очікує' або 'Скасовано'");
            }

            // Видаляємо пов'язані OrderItems
            var orderItems = await _unitOfWork.OrderItemRepository.GetAsync(
                oi => oi.OrderId == id,
                null,
                ""
            );

            _unitOfWork.OrderItemRepository.RemoveRange(orderItems);
            await _unitOfWork.OrderRepository.RemoveAsync(id);
            await _unitOfWork.SaveAsync();
        }
    }
}