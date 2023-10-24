using Gameapp.Application.Contracts;
using Gameapp.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Gameapp.Application.Features.Items.Queries.GetAllItems;

internal sealed class GetAllItemsQueryHandler : IRequestHandler<GetAllItemsQuery, IEnumerable<Item>>
{
    private readonly IApplicationDbContext _context;

    public GetAllItemsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Item>> Handle(GetAllItemsQuery request, CancellationToken cancellationToken)
    {
        return await _context.Items.ToListAsync(cancellationToken);
    }
}
