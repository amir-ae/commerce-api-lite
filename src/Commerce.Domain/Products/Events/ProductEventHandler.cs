namespace Commerce.Domain.Products.Events;

public class ProductEventHandler<TEvent> where TEvent : ProductEvent
{
    public event Action<TEvent>? EventOccurred;

    public void RaiseEvent(TEvent eventArgs)
    {
        EventOccurred?.Invoke(eventArgs);
    }
}