using FluentValidation;
using FluentValidation.Results;
using Gameapp.Application.Contracts;
using Gameapp.Application.Exceptions.Items;
using Gameapp.Application.Features.Items.Queries.GetItemById;
using Gameapp.Domain.Entities;
using Gameapp.Domain.Repositories;
using MediatR;

namespace Gameapp.Application.Features.Items.Commands.DeleteItem;

internal sealed class DeleteItemCommandHandler : IRequestHandler<DeleteItemCommand, Unit>
{
    private readonly IMediator _mediator;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<DeleteItemCommand> _validator;
    private readonly IItemRepository _itemRepository;

    public DeleteItemCommandHandler(
        IMediator mediator, 
        IUnitOfWork unitOfWork, 
        IValidator<DeleteItemCommand> validator, 
        IItemRepository itemRepository)
    {
        _mediator = mediator;
        _unitOfWork = unitOfWork;
        _validator = validator;
        _itemRepository = itemRepository;
    }

    public async Task<Unit> Handle(DeleteItemCommand request, CancellationToken cancellationToken)
    {
        ValidationResult validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        Item item = await _mediator.Send(new GetItemByIdQuery { Id = request.Id }, cancellationToken);

        if (item == null)
        {
            throw new NotFoundException(nameof(Item), request.Id);
        }

        _itemRepository.Delete(item);
        await _unitOfWork.SaveChangesAsync();

        return Unit.Value;
    }
}
