using FluentValidation;

namespace Gameapp.Application.Features.Items.Queries.GetItemById;

public class GetItemByIdQueryValidator : AbstractValidator<GetItemByIdQuery>
{
    public GetItemByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .Must(id => id != Guid.Empty)
            .WithMessage("Id não pode ser vazio.");
        RuleFor(x => x.Id)
            .Must(id => !string.IsNullOrEmpty(id.ToString()))
            .WithMessage("Id não pode ser nulo ou uma string vazia.");
    }
}
