using FluentValidation;

namespace Application.Features.Notes.Commands;
public class CreateNoteCommandValidator : AbstractValidator<CreateNoteCommand>
{
    public CreateNoteCommandValidator()
    {
        RuleFor(x => x.Title)
            .MaximumLength(100).WithMessage("Title cannot be longer than 100 characters");

        RuleFor(x => x.Content)
            .MaximumLength(100).WithMessage("Note cannot contain more than 1000 characters");
    }
}