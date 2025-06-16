using FoodDelivery.DAL.Data;
using FoodDelivery.DAL.Entities;
using FoodDelivery.DAL.Interfaces;
using System;
using System.Threading.Tasks;

namespace FoodDelivery.DAL.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private IRepository<Restaurant> _restaurantRepository;
        private IRepository<MenuItem> _menuItemRepository;
        private IRepository<Category> _categoryRepository;
        private IRepository<Order> _orderRepository;
        private IRepository<OrderItem> _orderItemRepository;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public IRepository<Restaurant> RestaurantRepository
        {
            get
            {
                if (_restaurantRepository == null)
                {
                    _restaurantRepository = new Repository<Restaurant>(_context);
                }
                return _restaurantRepository;
            }
        }

        public IRepository<MenuItem> MenuItemRepository
        {
            get
            {
                if (_menuItemRepository == null)
                {
                    _menuItemRepository = new Repository<MenuItem>(_context);
                }
                return _menuItemRepository;
            }
        }

        public IRepository<Category> CategoryRepository
        {
            get
            {
                if (_categoryRepository == null)
                {
                    _categoryRepository = new Repository<Category>(_context);
                }
                return _categoryRepository;
            }
        }

        public IRepository<Order> OrderRepository
        {
            get
            {
                if (_orderRepository == null)
                {
                    _orderRepository = new Repository<Order>(_context);
                }
                return _orderRepository;
            }
        }

        public IRepository<OrderItem> OrderItemRepository
        {
            get
            {
                if (_orderItemRepository == null)
                {
                    _orderItemRepository = new Repository<OrderItem>(_context);
                }
                return _orderItemRepository;
            }
        }

        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}