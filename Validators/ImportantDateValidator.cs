using FluentValidation;
using PlanIT.API.Models.DTOs;

namespace PlanIT.API.Validators;

public class ImportantDateValidator : AbstractValidator<ImportantDateDTO>
{

    public ImportantDateValidator()
    {

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("ImportantDate Name cannot be empty")
            .MinimumLength(3).WithMessage("ImportantDate Name must be a least 3 characters ")
            .MaximumLength(50).WithMessage("ImportantDate Name cannot exceed 50 charcters");



        // Not sure if needed
        RuleFor(x => x.Date)
            .NotEmpty().WithMessage("ImportantDate Date cannot be empty");


    }


}
