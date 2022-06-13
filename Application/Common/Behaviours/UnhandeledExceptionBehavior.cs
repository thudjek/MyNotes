using Application.Common.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Common.Behaviours;
public class UnhandeledExceptionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly ILogger<UnhandeledExceptionBehavior<TRequest, TResponse>> _logger;

    public UnhandeledExceptionBehavior(ILogger<UnhandeledExceptionBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
    {
        var ignoreLoggingExceptions = new List<Type>() { typeof(ValidationException) };
        var requestName = typeof(TRequest).Name;

        try
        {
            return await next();
        }
        catch (Exception ex)
        {
            var exceptionType = ex.GetType();

            if (!ignoreLoggingExceptions.Contains(exceptionType))
            {
                switch (ex)
                {
                    case IdentityException identityException:
                        _logger.LogError(ex, "Unhandeled exception of type {ExceptionType} occurred while processing request {RequestName}. {@Errors}", exceptionType.Name, requestName, new { identityException.Errors });
                        break;
                    default:
                        _logger.LogError(ex, "Unhandeled exception of type {ExceptionType} occurred while processing request {RequestName}.", exceptionType.Name, requestName);
                        break;
                } 
            }
                
            throw;
        }
    }
}
