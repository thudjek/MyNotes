using SharedModels;

namespace WebApp.Common.Exceptions;

public class ValidationException : Exception
{
    public ValidationException(ErrorModel errorModel)
    {
        Errors = errorModel.Errors;
        ErrorsGrouped = errorModel.ErrorsGrouped;
    }
    public string[] Errors { get; }
    public Dictionary<string, string[]> ErrorsGrouped { get; }
}
