using AutoMapper;
using FoodDelivery.Data.Entities;
using FoodDelivery.Models.DTOs;
using System.Runtime;

namespace FoodDelivery.API.Mappings
{
    public class ItemMappingProfile : Profile
    {
        public ItemMappingProfile()
        {
            CreateMap<Item, ItemDto>();
        }
    }
}
