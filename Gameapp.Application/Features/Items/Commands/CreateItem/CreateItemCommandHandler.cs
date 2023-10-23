using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Gameapp.Application.Contracts;
using Gameapp.Domain.Entities;
using Gameapp.Domain.Repositories;
using MediatR;

namespace Gameapp.Application.Features.Items.Commands.CreateItem;

public class CreateItemCommandHandler : IRequestHandler<CreateItemCommand, Item>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateItemCommand> _validator;
    private readonly IItemRepository _itemRepository;

    public CreateItemCommandHandler(
        IMapper mapper, 
        IUnitOfWork unitOfWork, 
        IValidator<CreateItemCommand> validator, 
        IItemRepository itemRepository)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _validator = validator;
        _itemRepository = itemRepository;
    }

    public async Task<Item> Handle(CreateItemCommand request, CancellationToken cancellationToken)
    {
        ValidationResult validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        Item item = _mapper.Map<Item>(request);
        item.CreatedDate = DateTime.UtcNow;

        item = await _itemRepository.CreateAsync(item);
        await _unitOfWork.SaveChangesAsync();

        return item;
    }
}
