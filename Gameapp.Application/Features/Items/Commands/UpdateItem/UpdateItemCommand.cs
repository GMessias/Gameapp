using Gameapp.Domain.Entities;
using MediatR;

namespace Gameapp.Application.Features.Items.Commands.UpdateItem;

public class UpdateItemCommand : IRequest<Unit>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
