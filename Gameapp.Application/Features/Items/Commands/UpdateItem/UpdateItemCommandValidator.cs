using FluentValidation;

namespace Gameapp.Application.Features.Items.Commands.UpdateItem;

public class UpdateItemCommandValidator : AbstractValidator<UpdateItemCommand>
{
    public UpdateItemCommandValidator()
    {
        RuleFor(x => x.Id)
            .Must(id => id != Guid.Empty)
            .WithMessage("Id não pode ser vazio.");
        RuleFor(x => x.Id)
            .Must(id => !string.IsNullOrEmpty(id.ToString()))
            .WithMessage("Id não pode ser nulo ou uma string vazia.");

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
