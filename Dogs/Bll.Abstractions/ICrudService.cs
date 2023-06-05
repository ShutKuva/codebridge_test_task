using System.Linq.Expressions;

namespace Bll.Abstractions
{
    public interface ICrudService<TEntity, TIdentificator, TPage> 
        where TEntity : class
    {
        Task CreateAsync(TEntity entity);

        Task<IEnumerable<TEntity>> ReadAsync<TKey>(
            Expression<Func<TEntity, bool>> predicate,
            TPage page,
            Expression<Func<TEntity, TKey>>? orderByKey,
            bool? isDescending = null);

        Task<IEnumerable<TEntity>> ReadAsyncWithUnknownKeyInCompilationTime(
            Expression<Func<TEntity, bool>>? predicate,
            TPage page,
            LambdaExpression? orderByKey,
            Type? keyType,
            bool? isDescending = null);

        Task UpdateAsync(TEntity entity);

        Task DeleteAsync(TIdentificator id);
    }
}