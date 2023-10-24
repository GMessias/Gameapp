using FluentValidation;

namespace Gameapp.Application.Features.Items.Commands.DeleteItem;

public class DeleteItemCommandValidator : AbstractValidator<DeleteItemCommand>
{
    public DeleteItemCommandValidator()
    {
        RuleFor(x => x.Id)
            .Must(id => id != Guid.Empty)
            .WithMessage("Id não pode ser vazio.");
        RuleFor(x => x.Id)
            .Must(id => !string.IsNullOrEmpty(id.ToString()))
            .WithMessage("Id não pode ser nulo ou uma string vazia.");
    }
}
