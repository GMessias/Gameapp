using Gameapp.Domain.Entities;
using MediatR;

namespace Gameapp.Application.Features.Items.Queries.GetItemById;

public class GetItemByIdQuery : IRequest<Item>
{
    public Guid Id { get; set; }
}
