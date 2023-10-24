using Gameapp.Domain.Entities;
using MediatR;

namespace Gameapp.Application.Features.Items.Queries.GetAllItems;

public sealed class GetAllItemsQuery : IRequest<IEnumerable<Item>>
{
}
