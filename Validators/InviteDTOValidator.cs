using FluentValidation;
using PlanIT.API.Models.DTOs;

namespace PlanIT.API.Validators;

public class InviteDTOValidator : AbstractValidator<InviteDTO>
{
    public InviteDTOValidator()
    {
        RuleFor(x => x.Email)
             .EmailAddress().WithMessage("Valid email is required")
             .NotEmpty().WithMessage("User Email cannot be empty")
             .MaximumLength(100).WithMessage(" User Email cannot exceed 100 characters");



        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("User Name cannot be empty")
            .MinimumLength(2).WithMessage("User Name must be a least 2 characters")
            .MaximumLength(50).WithMessage(" User Name cannot exceed 50 characters");

    }

}


