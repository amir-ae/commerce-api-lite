namespace Commerce.API.Extensions.ETag;

public static class EnumerableExtensions {
    public static int GetCombinedHashCode<T>(this IEnumerable<T> source) => 
        source.Aggregate(typeof(T).GetHashCode(), HashCode.Combine);
}