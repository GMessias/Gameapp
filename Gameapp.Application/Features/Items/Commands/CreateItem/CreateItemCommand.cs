using Gameapp.Domain.Entities;
using MediatR;

namespace Gameapp.Application.Features.Items.Commands.CreateItem;

public class CreateItemCommand : IRequest<Item>
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
