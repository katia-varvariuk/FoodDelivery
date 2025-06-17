using FoodDelivery.BLL.DTOs;
using FoodDelivery.BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using FoodDelivery.API.Filters;

namespace FoodDelivery.API.Controllers
{
    /// <summary>
    /// API для управління ресторанами
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class RestaurantsController : ControllerBase
    {
        private readonly IRestaurantService _restaurantService;
        private readonly IMenuItemService _menuItemService;

        public RestaurantsController(IRestaurantService restaurantService, IMenuItemService menuItemService)
        {
            _restaurantService = restaurantService;
            _menuItemService = menuItemService;
        }

        /// <summary>
        /// Отримати список ресторанів з фільтрацією та пагінацією
        /// </summary>
        /// <param name="filterDto">Параметри фільтрації та сортування</param>
        /// <returns>Список ресторанів з пагінацією</returns>
        /// <response code="200">Повертає список ресторанів</response>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResultDto<RestaurantDto>), 200)]
        public async Task<IActionResult> GetAllRestaurants([FromQuery] RestaurantFilterDto filterDto)
        {
            var restaurants = await _restaurantService.GetAllAsync(filterDto);
            return Ok(restaurants);
        }

        /// <summary>
        /// Отримати ресторан за ідентифікатором
        /// </summary>
        /// <param name="id">Ідентифікатор ресторану</param>
        /// <returns>Дані ресторану</returns>
        /// <response code="200">Повертає дані ресторану</response>
        /// <response code="404">Ресторан не знайдено</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(RestaurantDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetRestaurantById(int id)
        {
            var restaurant = await _restaurantService.GetByIdAsync(id);

            if (restaurant == null)
            {
                return NotFound();
            }

            return Ok(restaurant);
        }

        /// <summary>
        /// Отримати меню ресторану
        /// </summary>
        /// <param name="id">Ідентифікатор ресторану</param>
        /// <returns>Список страв ресторану</returns>
        /// <response code="200">Повертає список страв</response>
        /// <response code="404">Ресторан не знайдено</response>
        [HttpGet("{id}/menu-items")]
        [ProducesResponseType(typeof(IEnumerable<MenuItemDto>), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetRestaurantMenuItems(int id)
        {
            var restaurant = await _restaurantService.GetByIdAsync(id);

            if (restaurant == null)
            {
                return NotFound();
            }

            var menuItems = await _menuItemService.GetAllByRestaurantIdAsync(id);
            return Ok(menuItems);
        }

        /// <summary>
        /// Створити новий ресторан
        /// </summary>
        /// <param name="restaurantDto">Дані ресторану</param>
        /// <returns>Створений ресторан</returns>
        /// <response code="201">Ресторан успішно створено</response>
        /// <response code="400">Помилка валідації даних</response>
        /// <response code="401">Користувач не авторизований</response>
        /// <response code="403">Доступ заборонено</response>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateModelState]
        [ProducesResponseType(typeof(RestaurantDto), 201)]
        [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> CreateRestaurant([FromBody] RestaurantDto restaurantDto)
        {
            var createdRestaurant = await _restaurantService.CreateAsync(restaurantDto);
            return CreatedAtAction(nameof(GetRestaurantById), new { id = createdRestaurant.Id }, createdRestaurant);
        }

        /// <summary>
        /// Оновити інформацію про ресторан
        /// </summary>
        /// <param name="id">Ідентифікатор ресторану</param>
        /// <param name="restaurantDto">Нові дані ресторану</param>
        /// <returns>Оновлений ресторан</returns>
        /// <response code="200">Ресторан успішно оновлено</response>
        /// <response code="400">Помилка валідації даних</response>
        /// <response code="401">Користувач не авторизований</response>
        /// <response code="403">Доступ заборонено</response>
        /// <response code="404">Ресторан не знайдено</response>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Restaurant")]
        [ValidateModelState]
        [ProducesResponseType(typeof(RestaurantDto), 200)]
        [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateRestaurant(int id, [FromBody] RestaurantDto restaurantDto)
        {
            var updatedRestaurant = await _restaurantService.UpdateAsync(id, restaurantDto);

            if (updatedRestaurant == null)
            {
                return NotFound();
            }

            return Ok(updatedRestaurant);
        }

        /// <summary>
        /// Видалити ресторан
        /// </summary>
        /// <param name="id">Ідентифікатор ресторану</param>
        /// <returns>Результат видалення</returns>
        /// <response code="204">Ресторан успішно видалено</response>
        /// <response code="401">Користувач не авторизований</response>
        /// <response code="403">Доступ заборонено</response>
        /// <response code="404">Ресторан не знайдено</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteRestaurant(int id)
        {
            var restaurant = await _restaurantService.GetByIdAsync(id);

            if (restaurant == null)
            {
                return NotFound();
            }

            await _restaurantService.DeleteAsync(id);
            return NoContent();
        }
    }
}