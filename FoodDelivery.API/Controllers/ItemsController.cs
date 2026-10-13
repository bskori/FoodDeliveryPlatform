using AutoMapper;
using FoodDelivery.Data;
using FoodDelivery.Data.Entities;
using FoodDelivery.Data.Repositories.Interfaces;
using FoodDelivery.Models.DTOs;
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
        private readonly IMapper _mapper;
        public ItemsController(IGenricRepository<Item> itemRepository, IMapper mapper)
        {
            _itemRepository = itemRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllItems(CancellationToken cancellationToken)
        {
            var items = await _itemRepository
                .GetAllAsync(cancellationToken);

            var itemDtos = _mapper.Map<List<ItemDto>>(items);

            return Ok(itemDtos);
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

            var itemDto = _mapper.Map<ItemDto>(item);

            return Ok(itemDto);
        }


    }
}
