using Application.Common.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class Repository<T> : IRepository<T>
    where T : BaseEntity
{
    private readonly ArticleDbContext _dbContext;
    private readonly DbSet<T> _set;

    public Repository(ArticleDbContext dbContext)
    {
        _dbContext = dbContext;
        _set = _dbContext.Set<T>();
    }

    public IQueryable<T> Table => _set;

    public async Task Insert(T entity, CancellationToken cancellationToken = default)
    {
        await _set.AddAsync(entity, cancellationToken);
    }

    public void Remove(T entity)
    {
        _set.Remove(entity);
    }

    public async Task SaveAll(CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
