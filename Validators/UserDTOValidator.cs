using FluentValidation;
using PlanIT.API.Models.DTOs;
using System.Text.RegularExpressions;

namespace PlanIT.API.Validators;

public class UserDTOValidator : AbstractValidator<UserDTO>
{

    public UserDTOValidator()
    {
        RuleFor(x => x.Name)
           .NotEmpty().WithMessage("User Name cannot be empty")
           .MinimumLength(2).WithMessage("User Name must be a least 2 characters")
           .MaximumLength(50).WithMessage("User Name cannot exceed 100 characters");


        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("User email cannot be empty")
            .MaximumLength(100).WithMessage("Email cannot exceed 100 characters")
            .Must(BeAValidEmail).WithMessage("Valid email is required, including domain and extension");

    }

    private bool BeAValidEmail(string email)
    {
        // checks that the email has atleast 1 . after the @
        return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
    }
}

}
