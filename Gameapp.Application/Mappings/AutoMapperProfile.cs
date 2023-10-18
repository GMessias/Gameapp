using AutoMapper;
using Gameapp.Application.Features.Items.Commands.CreateItem;
using Gameapp.Domain.Entities;

namespace Gameapp.Application.Mappings;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<CreateItemCommand, Item>();
    }
}
