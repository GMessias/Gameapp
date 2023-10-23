using FluentValidation;

namespace Gameapp.Application.Features.Items.Commands.CreateItem;

public class CreateItemCommandValidator : AbstractValidator<CreateItemCommand>
{
    public CreateItemCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name não pode ser vazio.");
        RuleFor(x => x.Name)
            .NotNull()
            .WithMessage("Name não pode ser nulo.");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Description não pode ser vazio.");
        RuleFor(x => x.Description)
            .NotNull()
            .WithMessage("Description não pode ser nulo.");
    }
}
