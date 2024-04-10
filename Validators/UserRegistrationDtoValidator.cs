using FluentValidation;
using PlanIT.API.Models.DTOs;

namespace PlanIT.API.Validators;

public class UserRegistrationDtoValidator : AbstractValidator<UserRegDTO>
{
    public UserRegistrationDtoValidator()
    {

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("User Name cannot be empty")
            .MinimumLength(2).WithMessage("User Name must be a least 2 characters")
            .MaximumLength(50).WithMessage(" User Name cannot exceed 50 characters");


        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("User email cannot be empty")
            .EmailAddress().WithMessage("Valid email is required")
            .MaximumLength(100).WithMessage(" User Name cannot exceed 100 characters");


        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password cannot be empty")
            .MinimumLength(8).WithMessage("Password must be atleast 8 characters")
            .MaximumLength(16).WithMessage("Password cannot exceed  16 characters")
            .Matches(@"[0-9]+").WithMessage("Password needs atleast 1 number")
            .Matches(@"[A-z]+").WithMessage("Password needs atleast 1 uppercase letter")
            .Matches(@"[a-z]+").WithMessage("Password needs atleast 1 lowercase letter")
            .Matches(@"[!?*#_]+").WithMessage("Password needs atleast 1 special character (! ? * # _)");

    }
}
