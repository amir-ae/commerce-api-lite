namespace Commerce.Domain.Common.Models;

public class StronglyTypedId<T> : IEquatable<StronglyTypedId<T>> where T: IComparable<T>
{
    public T Value { get; }

    public StronglyTypedId(T value)
    {
        Value = value;
    }

    public bool Equals(StronglyTypedId<T>? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return EqualityComparer<T>.Default.Equals(Value, other.Value);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((StronglyTypedId<T>)obj);
    }

    public override int GetHashCode()
    {
        return EqualityComparer<T>.Default.GetHashCode(Value);
    }

    public static bool operator ==(StronglyTypedId<T>? left, StronglyTypedId<T>? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(StronglyTypedId<T>? left, StronglyTypedId<T>? right)
    {
        return !Equals(left, right);
    }
}