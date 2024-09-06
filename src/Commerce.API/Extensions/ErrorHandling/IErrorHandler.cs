using ErrorOr;

namespace Commerce.API.Extensions.ErrorHandling;

public interface IErrorHandler
{
    public IResult Problem(List<Error> errors);
    public IResult Problem(Error error);
}