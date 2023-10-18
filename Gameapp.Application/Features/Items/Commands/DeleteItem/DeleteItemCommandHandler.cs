using Gameapp.Application.Exceptions.Items;
using Gameapp.Application.Features.Items.Queries.GetItemById;
using Gameapp.Domain.Entities;
using Gameapp.Domain.Repositories;
using MediatR;

namespace Gameapp.Application.Features.Items.Commands.DeleteItem;

public class DeleteItemCommandHandler : IRequestHandler<DeleteItemCommand, Unit>
{
    private readonly IMediator _mediator;
    private readonly IItemRepository _itemRepository;

    public DeleteItemCommandHandler(IMediator mediator, IItemRepository itemRepository)
    {
        _mediator = mediator;
        _itemRepository = itemRepository;
    }

    public async Task<Unit> Handle(DeleteItemCommand request, CancellationToken cancellationToken)
    {
        // Validar

        Item item = await _mediator.Send(new GetItemByIdQuery { Id = request.Id }, cancellationToken);

        if (item == null)
        {
            throw new NotFoundException(nameof(Item), request.Id);
        }

        await _itemRepository.DeleteAsync(item);

        return Unit.Value;
    }
}
