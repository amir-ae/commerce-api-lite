using System.Reflection;
using Mapster;
using Marten.Events;

namespace Commerce.Infrastructure.Common.Extensions;

public static class EventsExtensions
{
    public static T Sort<T>(this List<IEvent> events, T target)
    {
        var properties = typeof(T).GetProperties(
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

        foreach (var prop in properties)
        {
            if (!prop.PropertyType.IsGenericType) continue;
            var propType = prop.PropertyType.GetGenericArguments().Single();
            var listType = typeof(List<>).MakeGenericType(propType);
            var methodInfo = typeof(EventsExtensions).GetMethod(nameof(ListTypedEvents))!.MakeGenericMethod(propType);
            var sourceList = methodInfo.Invoke(null, new object[] { events });
            prop.SetValue(target, Activator.CreateInstance(listType, sourceList));
        }
        
        return target;
    }
    
    public static List<T> ListTypedEvents<T>(List<IEvent> events)
    {
        var constructedListType = typeof(List<>).MakeGenericType(typeof(T));
        var list = (List<T>)Activator.CreateInstance(constructedListType)!;
    
        foreach (var @event in events)
        {
            if (@event.EventType == typeof(T))
            {
                list.Add(@event.Data.Adapt<T>());
            }
        }
        return list;
    }
}