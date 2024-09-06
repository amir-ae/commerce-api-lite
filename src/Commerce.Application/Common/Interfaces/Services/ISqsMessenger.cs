using Amazon.SQS.Model;

namespace Commerce.Application.Common.Interfaces.Services;

public interface ISqsMessenger
{
    Task<SendMessageResponse> SendMessageAsync<T>(T message);
}