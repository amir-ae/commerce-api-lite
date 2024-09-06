using System.Collections.Concurrent;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Options;
using Commerce.Application.Common.Interfaces.Services;
using Commerce.Domain.Common.Configurations;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Commerce.Infrastructure.Common.Services;

public class SqsMessenger : ISqsMessenger
{
    private readonly IAmazonSQS _sqs;
    private readonly IOptions<QueueSettings> _queueSettings;
    private ConcurrentDictionary<string, string> _queueUrlCache;

    public SqsMessenger(IAmazonSQS sqs, IOptions<QueueSettings> queueSettings)
    {
        _sqs = sqs;
        _queueSettings = queueSettings;
        _queueUrlCache = new ConcurrentDictionary<string, string>();
    }

    public async Task<SendMessageResponse> SendMessageAsync<T>(T message)
    {

        var queueUrl = await QueueUrlAsync(message);

        var sendMessageRequest = new SendMessageRequest
        {
            QueueUrl = queueUrl,
            MessageBody = JsonSerializer.Serialize(message),
            MessageAttributes = new Dictionary<string, MessageAttributeValue>
            {
                {
                    "MessageType", new MessageAttributeValue
                    {
                        DataType = "String",
                        StringValue = typeof(T).Name
                    }
                }
            }
        };

        return await _sqs.SendMessageAsync(sendMessageRequest);
    }

    private async Task<string> QueueUrlAsync<T>(T message)
    {
        string key = message!.GetType().Name;

        if (_queueUrlCache.TryGetValue(key, out string? url))
        {
            return url;
        }

        var queueName = _queueSettings.Value[key]?.ToString();

        var queueUrlResponse = await _sqs.GetQueueUrlAsync(queueName);
        var queueUrl = queueUrlResponse.QueueUrl;

        _queueUrlCache.AddOrUpdate(key, queueUrl, (key, oldValue) => queueUrl);
        return queueUrl;
    }
}
