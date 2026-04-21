using FluentValidation;
using TaskManager.Api.Common;
using TaskManager.Api.Contracts;

namespace TaskManager.Api.Validators;

public class CreateTaskRequestValidator : AbstractValidator<CreateTaskRequest>
{
    public CreateTaskRequestValidator(ITimeService timeService)
    {
        ArgumentNullException.ThrowIfNull(timeService);

        RuleFor(x => x.Title).NotEmpty().MaximumLength(255);
        RuleFor(x => x.Notes).MaximumLength(4000).When(x => x.Notes is not null);
        RuleFor(x => x.Priority).IsInEnum().When(x => x.Priority.HasValue);
        RuleFor(x => x.DueDate)
            .Must(d => d >= timeService.GetTodayInDefaultTimezone())
            .WithName("Due Date")
            .WithMessage("'{PropertyName}' must be today or in the future.")
            .When(x => x.DueDate.HasValue);
    }
}
