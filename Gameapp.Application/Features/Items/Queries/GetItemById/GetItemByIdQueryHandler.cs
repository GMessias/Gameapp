using FluentValidation;
using FluentValidation.Results;
using Gameapp.Application.Contracts;
using Gameapp.Application.Exceptions.Items;
using Gameapp.Domain.Entities;
using MediatR;

namespace Gameapp.Application.Features.Items.Queries.GetItemById;

internal sealed class GetItemByIdQueryHandler : IRequestHandler<GetItemByIdQuery, Item>
{
    private readonly IApplicationDbContext _context;
    private readonly IValidator<GetItemByIdQuery> _validator;

    public GetItemByIdQueryHandler(
        IApplicationDbContext context,
        IValidator<GetItemByIdQuery> validator)
    {
        _context = context;
        _validator = validator;
    }

    public async Task<Item> Handle(GetItemByIdQuery request, CancellationToken cancellationToken)
    {
        ValidationResult validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        Item? item = await _context.Items.FindAsync(request.Id);

        if (item == null)
        {
            throw new NotFoundException(nameof(Item), request.Id);
        }

        return item;
    }
}
