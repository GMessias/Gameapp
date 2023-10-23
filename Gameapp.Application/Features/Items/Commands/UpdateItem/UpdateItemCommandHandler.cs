using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Gameapp.Application.Contracts;
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
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<UpdateItemCommand> _validator;
    private readonly IItemRepository _itemRepository;

    public UpdateItemCommandHandler(
        IMapper mapper, 
        IMediator mediator, 
        IUnitOfWork unitOfWork, 
        IValidator<UpdateItemCommand> validator, 
        IItemRepository itemRepository)
    {
        _mapper = mapper;
        _mediator = mediator;
        _unitOfWork = unitOfWork;
        _validator = validator;
        _itemRepository = itemRepository;
    }

    public async Task<Unit> Handle(UpdateItemCommand request, CancellationToken cancellationToken)
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

        item.Name = string.IsNullOrEmpty(request.Name) ? item.Name : request.Name;
        item.Description = string.IsNullOrEmpty(request.Description) ? item.Description : request.Description;
        item.UpdatedDate = DateTime.UtcNow;

        _itemRepository.Update(item);
        await _unitOfWork.SaveChangesAsync();

        return Unit.Value;
    }
}
