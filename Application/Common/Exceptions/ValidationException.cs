using FluentValidation.Results;

namespace Application.Common.Exceptions;
public class ValidationException : Exception
{
    public ValidationException() : base("One or more validations errors has occurred.")
    {

    }

    public ValidationException(IEnumerable<ValidationFailure> failures)
        : this()
    {
        ErrorsGrouped = failures
            .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
            .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());

        Errors = failures.Select(e => e.ErrorMessage).ToArray();
    }

    public string[] Errors { get; }
    public Dictionary<string, string[]> ErrorsGrouped { get; }
    
}