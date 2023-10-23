using FluentValidation;

namespace Gameapp.Application.Features.Items.Queries.GetItemById;

public class GetItemByIdQueryValidator : AbstractValidator<GetItemByIdQuery>
{
    public GetItemByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .Must(id => id != Guid.Empty)
            .WithMessage("Id não pode ser vazio.");
    }
}
