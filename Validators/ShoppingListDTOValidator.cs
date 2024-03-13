using FluentValidation;
using PlanIT.API.Models.DTOs;

namespace PlanIT.API.Validators;

public class ShoppingListDTOValidator : AbstractValidator<ShoppingListDTO>
{

    public ShoppingListDTOValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("ShoppingList Name cannot be empty")
            .MaximumLength(50).WithMessage("ShoppingList Name cannot exceed 50 characters")
            .MinimumLength(3).WithMessage("ShoppingList Name must be a least 3 characters");
    }
}
