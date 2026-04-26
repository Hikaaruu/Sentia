using System.Runtime.CompilerServices;
using System.Threading.Channels;
using Sentia.Application.Common.Interfaces;
using Sentia.Application.Features.Messages.Events;

namespace Sentia.Infrastructure.Cognitive.Services;

public sealed class SentimentAnalysisQueue : ISentimentAnalysisQueue
{
    private readonly Channel<MessageCreatedEvent> _channel =
        Channel.CreateUnbounded<MessageCreatedEvent>(new UnboundedChannelOptions
        {
            SingleReader = true
        });

    public ValueTask QueueAsync(MessageCreatedEvent item)
    {
        _channel.Writer.TryWrite(item);
        return ValueTask.CompletedTask;
    }

    public async IAsyncEnumerable<MessageCreatedEvent> DequeueAsync(
        [EnumeratorCancellation] CancellationToken ct)
    {
        await foreach (var item in _channel.Reader.ReadAllAsync(ct))
        {
            yield return item;
        }
    }
}
