using SharedModels;

namespace Application.Common;

public class Result
{
    public Result(bool isSuccess, string message)
    {
        IsSuccess = isSuccess;
        if(IsSuccess)
            SuccessMessage = message;
        else
            Error = message;
    }

    public Result(bool isSuccess)
    {
        IsSuccess = isSuccess;
    }

    public bool IsSuccess { get; protected set; }
    public string SuccessMessage { get; protected set; }
    public string Error { get; protected set; }

    public static Result Success()
    {
        return new Result(true);
    }

    public static Result Success(string message)
    {
        return new Result(true, message);
    }

    public static Result Failure()
    {
        return new Result(false);
    }

    public static Result Failure(string message)
    {
        return new Result(false, message);
    }

    public ErrorModel ToErrorModel()
    {
        return new ErrorModel(Error);
    }
}

public class Result<T> : Result where T : class
{
    public Result(T value) : base(true)
    {
        Value = value;
    }

    public Result(T value, string successMessage) : base(true, successMessage)
    {
        Value = value;
    }

    public Result(string error) : base(false, error)
    {
    }

    public T Value { get; private set; }

    public static Result<T> Success(T value)
    {
        return new Result<T>(value);
    }

    public static Result<T> Success(T value, string successMessage)
    {
        return new Result<T>(value, successMessage);
    }

    public new static Result<T> Failure(string error)
    {
        return new Result<T>(error);
    }

    public new static Result<T> Failure()
    {
        return new Result<T>(string.Empty);
    }
}