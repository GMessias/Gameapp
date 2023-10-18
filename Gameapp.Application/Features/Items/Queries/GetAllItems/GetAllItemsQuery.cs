using Gameapp.Domain.Entities;
using MediatR;

namespace Gameapp.Application.Features.Items.Queries.GetAllItems;

public class GetAllItemsQuery : IRequest<IEnumerable<Item>>
{
}
