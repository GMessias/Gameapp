using Gameapp.Application.Contracts;
using Gameapp.Application.Exceptions.Items;
using Gameapp.Domain.Entities;
using MediatR;

namespace Gameapp.Application.Features.Items.Queries.GetItemById;

public class GetItemByIdQueryHandler : IRequestHandler<GetItemByIdQuery, Item>
{
    private readonly IApplicationDbContext _context;

    public GetItemByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Item> Handle(GetItemByIdQuery request, CancellationToken cancellationToken)
    {
        Item? item = await _context.Items.FindAsync(request.Id);

        if (item == null)
        {
            throw new NotFoundException(nameof(Item), request.Id);
        }

        return item;
    }
}
