using FluentValidation;
using PlanIT.API.Models.DTOs;

namespace PlanIT.API.Validators;

public class InviteDTOValidator : AbstractValidator<InviteDTO>
{
    public InviteDTOValidator()
    {
        RuleFor(x => x.Email)
             .EmailAddress().When(x => !string.IsNullOrEmpty(x.Email));

        RuleFor(x => x.Name)
            .MaximumLength(50).When(i => !string.IsNullOrEmpty(i.Name))
            .MinimumLength(3).When(i => !string.IsNullOrEmpty(i.Name));

    }

}


