using MediatR;

namespace Gameapp.Application.Features.Items.Commands.DeleteItem;

public class DeleteItemCommand : IRequest<Unit>
{
    public Guid Id { get; set; }
}
