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
            .NotEmpty().WithMessage("Event Time cannot be empty")
            .Must(BeInCorrectFormat).WithMessage("Event Time must be in the format HH:MM");


        RuleFor(x => x.Date)
            .NotEmpty().WithMessage("Event Date cannot be empty")
            .Must(BeAValidDateOnly).WithMessage("Event Date cannot be in the past");


        RuleFor(x => x.Location)
            .NotEmpty().WithMessage("Event Location cannot be empty")
            .MinimumLength(4).WithMessage("Event Location must be a least 3 characters")
            .MaximumLength(50).WithMessage("Event Location cannot exceed 100 charcters");


    }

    private bool BeAValidDateOnly(DateOnly date)
    {
 
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        return date >= today;
    }



    private bool BeInCorrectFormat(TimeOnly time)
    {
        // Convert time to string with HH:mm format
        var timeString = time.ToString("HH:mm");
        // Check if conversion adds extra characters or not
        return time.ToString().Equals(timeString);
    }

}


