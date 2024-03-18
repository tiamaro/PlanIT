using FluentValidation;
using PlanIT.API.Models.DTOs;

namespace PlanIT.API.Validators;

public class UserDTOValidator : AbstractValidator<UserDTO>
{

    public UserDTOValidator()
    {
        RuleFor(x => x.Name)
           .NotEmpty().WithMessage("User Name cannot be empty")
           .MinimumLength(2).WithMessage("User Name must be a least 2 characters")
           .MaximumLength(50).WithMessage(" User Name cannot exceed 50 characters");


        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("User email cannot be empty")
            .EmailAddress().WithMessage("Valid email is required");

    }
}
