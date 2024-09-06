using ErrorOr;

namespace Commerce.API.Extensions.ErrorHandling;

public static class TaskExtensions
{
    public static Task<ErrorOr<T>> DefaultIfCanceled<T>(this Task<ErrorOr<T>> @this)
    {
        return
            @this.ContinueWith
            (
                t =>
                {
                    if (t.IsCanceled)
                    {
                        return Error.Conflict(nameof(StatusCodes.Status499ClientClosedRequest), 
                            "The client has closed the connection while the server is still processing the request.");
                    }
                    return t.Result;
                }
            );
    }
}