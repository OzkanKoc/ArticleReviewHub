using Domain.Entities;

namespace Application.Common.Repositories;

public interface IRepository<T>
    where T : BaseEntity
{
    IQueryable<T> Table { get; }
    Task Insert(T entity, CancellationToken cancellationToken = default);
    void Remove(T entity);

    Task SaveAll(CancellationToken cancellationToken = default);
}
