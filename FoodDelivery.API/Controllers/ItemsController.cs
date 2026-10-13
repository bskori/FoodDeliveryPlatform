using FoodDelivery.Data;
using FoodDelivery.Data.Entities;
using FoodDelivery.Data.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodDelivery.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemsController : ControllerBase
    {
        private readonly IGenricRepository<Item> _itemRepository;
        public ItemsController(IGenricRepository<Item> itemRepository)
        {
            _itemRepository = itemRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllItems(CancellationToken cancellationToken)
        {
            var items = await _itemRepository
                .GetAllAsync(cancellationToken);

            return Ok(items);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetItemById(int id, CancellationToken cancellationToken)
        {
            if(id <= 0)
            {
                return BadRequest(new
                {
                    message = "Invalid food item ID."
                });
            }

            var item = await _itemRepository.GetByIdAsync(id, cancellationToken);

            if(item == null) 
            {
                return NotFound(new
                {
                    message = "Food item not found."
                });
            }

            return Ok(item);
        }


    }
}
