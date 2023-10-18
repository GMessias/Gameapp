using AutoMapper;
using Gameapp.Application.Exceptions.Items;
using Gameapp.Application.Features.Items.Queries.GetItemById;
using Gameapp.Domain.Entities;
using Gameapp.Domain.Repositories;
using MediatR;

namespace Gameapp.Application.Features.Items.Commands.UpdateItem;

public class UpdateItemCommandHandler : IRequestHandler<UpdateItemCommand, Unit>
{
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;
    private readonly IItemRepository _itemRepository;

    public UpdateItemCommandHandler(IMapper mapper, IMediator mediator, IItemRepository itemRepository)
    {
        _mapper = mapper;
        _mediator = mediator;
        _itemRepository = itemRepository;
    }

    public async Task<Unit> Handle(UpdateItemCommand request, CancellationToken cancellationToken)
    {
        // Validar

        Item item = await _mediator.Send(new GetItemByIdQuery { Id = request.Id }, cancellationToken);

        if (item == null)
        {
            throw new NotFoundException(nameof(Item), request.Id);
        }

        item.Name = string.IsNullOrEmpty(request.Name) ? item.Name : request.Name;
        item.Description = string.IsNullOrEmpty(request.Description) ? item.Description : request.Description;
        item.UpdatedDate = DateTime.UtcNow;

        await _itemRepository.UpdateAsync(item);

        return Unit.Value;
    }
}
