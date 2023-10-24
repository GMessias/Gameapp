using MediatR;

namespace Gameapp.Application.Features.Items.Commands.DeleteItem;

public sealed class DeleteItemCommand : IRequest<Unit>
{
    public Guid Id { get; set; }
}
