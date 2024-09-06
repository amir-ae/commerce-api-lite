using ErrorOr;
using Serilog;

namespace Commerce.API.Extensions.ErrorHandling;

public class ErrorHandler : IErrorHandler
{
    public IResult Problem(List<Error> errors)
    {
        if (errors.Count is 0)
        {
            return Results.Problem();
        }

        if (errors.All(error => error.Type == ErrorType.Validation))
        {
            return ValidationProblem(errors);
        }

        return Problem(errors[0]);
    }

    public IResult Problem(Error error)
    {
        var statusCode = error.Type switch
        {
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => error.Code switch
            {
                nameof(StatusCodes.Status412PreconditionFailed) => StatusCodes.Status412PreconditionFailed,
                nameof(StatusCodes.Status499ClientClosedRequest) => StatusCodes.Status499ClientClosedRequest,
                _ => StatusCodes.Status409Conflict
            },
            _ => StatusCodes.Status500InternalServerError
        };

        if (!string.IsNullOrWhiteSpace(error.Description))
        {
            Log.Error(error.Description);
        }
        
        return TypedResults.Problem(statusCode: statusCode, detail: error.Description);
    }

    private IResult ValidationProblem(List<Error> errors)
    {
        var dict = new Dictionary<string, string[]>();

        foreach (var error in errors)
        {
            var added = dict.TryAdd(error.Code, new [] { error.Description });
            if (!added && dict.TryGetValue(error.Code, out var errorArray))
            {
                dict[error.Code] = errorArray.Concat(new [] { error.Description }).ToArray();
            }
            Log.Error(error.Description);
        }

        return TypedResults.ValidationProblem(errors: dict);
    }
}