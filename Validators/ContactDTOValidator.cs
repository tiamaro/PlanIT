using FluentValidation;
using PlanIT.API.Models.DTOs;
using System.Text.RegularExpressions;

namespace PlanIT.API.Validators;

public class ContactDTOValidator : AbstractValidator<ContactDTO>
{

    public ContactDTOValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("User Email cannot be empty")
            .MaximumLength(100).WithMessage(" User Email cannot exceed 100 characters")
            .Must(BeAValidEmail).WithMessage("Valid email is required, including domain and extension");



        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("User Name cannot be empty")
            .MinimumLength(2).WithMessage("User Name must be a least 2 characters")
            .MaximumLength(50).WithMessage(" User Name cannot exceed 50 characters");
    }


    // Ensures that the email is in the correct format
    private bool BeAValidEmail(string email)
    {
        
        return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
    }

}
