using Domain.Entities;
using FluentValidation;

namespace Domain.Validators;

public class QuestionValidator : AbstractValidator<Question>
{
    public const string IdEmptyMessage = "El id no puede estar vacío";

    public const string TitleEmptyMessage = "El título no puede estar vacío";
    public static readonly string TitleMaxLengthMessage = $"El título no puede tener más de {Question.TitleField.TitleMaxLength} caracteres";

    public const string ContentEmptyMessage = "El contenido no puede estar vacío";
    public static readonly string ContentMaxLengthMessage = $"El contenido no puede tener más de {Question.ContentField.ContentMaxLength} caracteres";

    public QuestionValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage(IdEmptyMessage);

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage(TitleEmptyMessage)
            .MaximumLength(Question.TitleField.TitleMaxLength)
                .WithMessage(TitleMaxLengthMessage);

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage(ContentEmptyMessage)
            .MaximumLength(Question.ContentField.ContentMaxLength)
                .WithMessage(ContentMaxLengthMessage);
    }
}
