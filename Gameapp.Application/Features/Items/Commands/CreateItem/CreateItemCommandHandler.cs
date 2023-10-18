using AutoMapper;
using Gameapp.Domain.Entities;
using Gameapp.Domain.Repositories;
using MediatR;

namespace Gameapp.Application.Features.Items.Commands.CreateItem;

public class CreateItemCommandHandler : IRequestHandler<CreateItemCommand, Item>
{
    private readonly IMapper _mapper; 
    private readonly IItemRepository _itemRepository;

    public CreateItemCommandHandler(IMapper mapper, IItemRepository itemRepository)
    {
        _mapper = mapper;
        _itemRepository = itemRepository;
    }

    public async Task<Item> Handle(CreateItemCommand request, CancellationToken cancellationToken)
    {
        // Validar o request

        Item item = _mapper.Map<Item>(request);
        item.CreatedDate = DateTime.UtcNow;

        item = await _itemRepository.CreateAsync(item);

        return item;
    }
}
