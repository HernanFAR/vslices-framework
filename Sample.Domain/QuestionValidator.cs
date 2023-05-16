using FluentValidation;

namespace Sample.Domain;

public class QuestionValidator : AbstractValidator<Question>
{
    public QuestionValidator()
    {
        RuleFor(e => e.Id)
            .NotEmpty();

        RuleFor(e => e.Name)
            .NotEmpty();

        RuleFor(e => e.CreatedById)
            .NotEmpty();

        RuleFor(e => e.UpdatedById)
            .Must((q, id) => id is null || q.CreatedById == id);

        RuleFor(e => e.DeleteById)
            .Must((q, id) => id is null || q.CreatedById == id);

    }
}
