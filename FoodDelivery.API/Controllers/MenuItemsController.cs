using FoodDelivery.BLL.DTOs;
using FoodDelivery.BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FoodDelivery.API.Controllers
{
    [Route("api/menu-items")]
    [ApiController]
    public class MenuItemsController : ControllerBase
    {
        private readonly IMenuItemService _menuItemService;

        public MenuItemsController(IMenuItemService menuItemService)
        {
            _menuItemService = menuItemService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMenuItemById(int id)
        {
            var menuItem = await _menuItemService.GetByIdAsync(id);

            if (menuItem == null)
            {
                return NotFound();
            }

            return Ok(menuItem);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Restaurant")]
        public async Task<IActionResult> CreateMenuItem([FromBody] MenuItemDto menuItemDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdMenuItem = await _menuItemService.CreateAsync(menuItemDto);
            return CreatedAtAction(nameof(GetMenuItemById), new { id = createdMenuItem.Id }, createdMenuItem);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Restaurant")]
        public async Task<IActionResult> UpdateMenuItem(int id, [FromBody] MenuItemDto menuItemDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedMenuItem = await _menuItemService.UpdateAsync(id, menuItemDto);

            if (updatedMenuItem == null)
            {
                return NotFound();
            }

            return Ok(updatedMenuItem);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Restaurant")]
        public async Task<IActionResult> DeleteMenuItem(int id)
        {
            var menuItem = await _menuItemService.GetByIdAsync(id);

            if (menuItem == null)
            {
                return NotFound();
            }

            await _menuItemService.DeleteAsync(id);
            return NoContent();
        }
    }
}