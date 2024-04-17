using FluentValidation;
using PlanIT.API.Models.DTOs;

namespace PlanIT.API.Validators;

public class EventDTOValidator : AbstractValidator<EventDTO>
{

    public EventDTOValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Event Name cannot be empty")
            .MinimumLength(3).WithMessage("Event Name must be a least 3 characters")
            .MaximumLength(50).WithMessage("Event Name cannot exceed 50 characters");


        RuleFor(x => x.Time)
            .NotEmpty().WithMessage("Event Time cannot be empty");


        RuleFor(x => x.Date)
            .NotEmpty().WithMessage("Event Date cannot be empty");


        RuleFor(x => x.Location)
            .NotEmpty().WithMessage("Event Location cannot be empty")
            .MinimumLength(4).WithMessage("Event Location must be a least 4 characters")
            .MaximumLength(50).WithMessage("Event Location cannot exceed 50 charcters");


    }



    private bool BeAValidTime(string time)
    {
        string pattern = @"^(0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$";


        if (System.Text.RegularExpressions.Regex.IsMatch(time, pattern))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

}


