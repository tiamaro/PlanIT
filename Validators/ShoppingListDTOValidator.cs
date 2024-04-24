using FluentValidation;
using PlanIT.API.Models.DTOs;

namespace PlanIT.API.Validators;

public class ShoppingListDTOValidator : AbstractValidator<ShoppingListDTO>
{

    public ShoppingListDTOValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Shoppinglist Name cannot be empty")
            .MaximumLength(50).WithMessage("Shoppinglist Name cannot exceed 50 characters")
            .MinimumLength(3).WithMessage("Shoppinglist Name must be a least 3 characters");
    }
}
