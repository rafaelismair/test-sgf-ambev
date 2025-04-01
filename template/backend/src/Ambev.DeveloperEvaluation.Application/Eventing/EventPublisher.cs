using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Eventing;
public interface IEventPublisher
{
    Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken) where TEvent : class;
}
public class EventPublisher : IEventPublisher
{
    private readonly ILogger<EventPublisher> _logger;

    public EventPublisher(ILogger<EventPublisher> logger)
    {
        _logger = logger;
    }

    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken) where TEvent : class
    {
        _logger.LogInformation("Event published: {EventName}, Data: {@EventData}", @event.GetType().Name, @event);

        await Task.CompletedTask;
    }
}