﻿using FluentValidation;
using PlanIT.API.Models.DTOs;

namespace PlanIT.API.Validators;

public class DinnerDTOValidator : AbstractValidator<DinnerDTO>
{
    public DinnerDTOValidator()
    {

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Dinner Name cannot be empty")
            .MinimumLength(3).WithMessage("Dinner Name must be a least 3 characters ")
            .MaximumLength(50).WithMessage("Dinner Name cannot exceed 50 charcters");



        // Not sure if needed
        RuleFor(x => x.Date)
            .NotEmpty().WithMessage("Dinner Date cannot be empty");

    }
}
