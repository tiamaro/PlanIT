using FluentValidation;
using PlanIT.API.Models.DTOs;

namespace PlanIT.API.Validators;

public class ToDoDTOValidator : AbstractValidator<ToDoDTO>
{

    public ToDoDTOValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("ToDo Name Cannot be empty")
            .MinimumLength(3).WithMessage("ToDo Name must be a least 3 characters")
            .MaximumLength(50).WithMessage("ToDo Name cannot exceed 50 characters");

    }
}
