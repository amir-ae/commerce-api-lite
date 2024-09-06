namespace Commerce.API.Contract.V1.Common.Responses;

public record PaginatedList<TEntity>(int PageNumber, 
    int PageSize, 
    long Total, 
    IList<TEntity> Data) where TEntity : class;