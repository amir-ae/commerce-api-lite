using System.Runtime.Serialization;
using Commerce.Domain.Common.Interfaces;
using Commerce.Domain.Common.ValueObjects;
using Marten.Metadata;

namespace Commerce.Domain.Common.Models;

public abstract class AggregateRoot<TId, T> : BaseEntity<TId>, IAuditable, IActivatable, ISoftDeletable, IRevisioned
    where TId: StronglyTypedId<T>
    where T : IComparable<T>
{
    [IgnoreDataMember]
    public T AggregateId    {
        get => Id.Value;
        protected set {}
    }
    
    public DateTimeOffset CreatedAt { get; protected set; } = DateTimeOffset.UtcNow;
    public AppUserId CreatedBy { get; protected set; } = new (new Guid());
    public DateTimeOffset? LastModifiedAt { get; protected set; }
    public AppUserId? LastModifiedBy { get; protected set; }
    public int Version { get; set; }
    public bool IsActive { get; protected set; } = true;
    public bool IsDeleted { get; protected set; }
}