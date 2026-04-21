using FluentValidation;
using TaskManager.Api.Contracts;

namespace TaskManager.Api.Validators;

public class TaskQueryFiltersValidator : AbstractValidator<TaskQueryFilters>
{
    public TaskQueryFiltersValidator()
    {
        RuleFor(x => x.Status).IsInEnum().When(x => x.Status.HasValue);
        RuleFor(x => x.Priority).IsInEnum().When(x => x.Priority.HasValue);
        RuleFor(x => x.DueDateFrom)
            .LessThanOrEqualTo(x => x.DueDateTo!.Value)
            .When(x => x.DueDateFrom.HasValue && x.DueDateTo.HasValue)
            .WithName("DueDateFrom")
            .WithMessage("'{PropertyName}' must be less than or equal to 'DueDateTo'.");
    }
}
