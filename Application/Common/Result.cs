namespace Application.Common;
public class Result<T>
{
    public Result(T value)
    {
        IsSuccess = true;
        Value = value;
    }

    public Result(T value, string successMessage)
    {
        IsSuccess = true;
        Value = value;
        SuccessMessage = successMessage;
    }

    public Result(string error)
    {
        IsSuccess = false;
        Error = error;
    }

    public T Value { get; private set; }
    public bool IsSuccess { get; private set; }
    public string SuccessMessage { get; private set; }
    public string Error { get; private set; }

    public static Result<T> Success(T value)
    {
        return new Result<T>(value);
    }

    public static Result<T> Success(T value, string successMessage)
    {
        return new Result<T>(value, successMessage);
    }

    public static Result<T> Failure(string error)
    {
        return new Result<T>(error);
    }

    public static Result<T> Failure()
    {
        return new Result<T>(string.Empty);
    }
}