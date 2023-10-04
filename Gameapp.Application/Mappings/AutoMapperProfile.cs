using AutoMapper;
using Gameapp.Application.Dtos;
using Gameapp.Application.Models;

namespace Gameapp.Application.Mappings;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Item, ItemDto>();
        CreateMap<ItemDto, Item>();
    }
}
