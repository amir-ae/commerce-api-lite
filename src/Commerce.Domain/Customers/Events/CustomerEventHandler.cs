namespace Commerce.Domain.Customers.Events;

public class CustomerEventHandler<TEvent> where TEvent : CustomerEvent
{
    public event Action<TEvent>? EventOccurred;

    public void RaiseEvent(TEvent eventArgs)
    {
        EventOccurred?.Invoke(eventArgs);
    }
}