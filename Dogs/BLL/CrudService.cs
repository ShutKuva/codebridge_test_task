using Bll.Abstractions;
using Core;
using Core.Db;
using DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;
using System.Reflection;

namespace BLL
{
    public class CrudService<T> : ICrudService<T, int, Page>
        where T : BaseEntity
    {
        private static Dictionary<Type, Func<IQueryable<T>, LambdaExpression, IQueryable<T>>> _orderBy;
        private static Dictionary<Type, Func<IQueryable<T>, LambdaExpression, IQueryable<T>>> _orderByDesc;

        private readonly DogsContext _context;

        static CrudService()
        {
            _orderBy = new Dictionary<Type, Func<IQueryable<T>, LambdaExpression, IQueryable<T>>>();
            _orderByDesc = new Dictionary<Type, Func<IQueryable<T>, LambdaExpression, IQueryable<T>>>();

            ParameterExpression instance = Expression.Parameter(typeof(IQueryable<T>), "instance");
            ParameterExpression orderKey = Expression.Parameter(typeof(LambdaExpression), "orderKey");

            foreach (PropertyInfo property in typeof(T).GetProperties())
            {
                if (_orderBy.ContainsKey(property.PropertyType))
                {
                    continue;
                }

                UnaryExpression orderKeyConvertExpression = Expression
                    .Convert(orderKey, typeof(Expression<>)
                    .MakeGenericType(
                        typeof(Func<,>).MakeGenericType(typeof(T), property.PropertyType)
                        ));

                _orderBy[property.PropertyType] = Expression.Lambda<Func<IQueryable<T>, LambdaExpression, IQueryable<T>>>(
                    Expression.Call(
                        typeof(Queryable)
                        .GetMethods()
                        .First(m => m.Name == "OrderBy" && m.GetParameters().Length == 2)
                        .MakeGenericMethod(typeof(T), property.PropertyType),
                        instance,
                        orderKeyConvertExpression),
                    instance,
                    orderKey)
                    .Compile();

                _orderByDesc[property.PropertyType] = Expression.Lambda<Func<IQueryable<T>, LambdaExpression, IQueryable<T>>>(
                    Expression.Call(
                        typeof(Queryable)
                        .GetMethods()
                        .First(m => m.Name == "OrderByDescending" && m.GetParameters().Length == 2 && m.GetParameters().All(p => p.ParameterType != typeof(IComparer<T>)))
                        .MakeGenericMethod(typeof(T), property.PropertyType),
                        instance,
                        orderKeyConvertExpression),
                    instance,
                    orderKey)
                    .Compile();
            }
        }

        public CrudService(DogsContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("Entity cannot be null.");
            }

            await _context.AddAsync(entity);

            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<T>> ReadAsync<TKey>(Expression<Func<T, bool>>? predicate, Page page, Expression<Func<T, TKey>>? orderByKey, bool? isDescending = null)
        {
            IQueryable<T> result = _context.Set<T>();

            if (predicate != null)
            {
                result = result.Where(predicate);
            }

            if (orderByKey != null)
            {
                if (isDescending != null)
                {
                    result = isDescending.Value ? result.OrderByDescending(orderByKey) : result.OrderBy(orderByKey);
                } else
                {
                    result = result.OrderBy(orderByKey);
                }
            }

            result = result.Page(page);

            return await result.ToListAsync();
        }

        public async Task<IEnumerable<T>> ReadAsyncWithUnknownKeyInCompilationTime(Expression<Func<T, bool>>? predicate, Page page, LambdaExpression? orderByKey, Type? keyType, bool? isDescending = null)
        {
            IQueryable<T> result = _context.Set<T>();

            if (predicate != null)
            {
                result = result.Where(predicate);
            }

            if (orderByKey != null)
            {
                Func<IQueryable<T>, LambdaExpression, IQueryable<T>> orderByFunc = null;

                if (isDescending != null)
                {
                    if (isDescending.Value)
                    {
                        orderByFunc = _orderByDesc[keyType];

                        result = orderByFunc.Invoke(result, orderByKey);
                    } 
                    else
                    {
                        orderByFunc = _orderBy[keyType];

                        result = orderByFunc.Invoke(result, orderByKey);
                    }
                }
                else
                {
                    orderByFunc = _orderBy[keyType];

                    result = orderByFunc.Invoke(result, orderByKey);
                }
            }

            result = result.Page(page);

            return await result.ToListAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("Entity cannot be null.");
            }

            T? oldValue = _context.Set<T>().FirstOrDefault(e => e.Id == entity.Id);

            if (oldValue == null)
            {
                _context.Add(entity);
            }
            else
            {
                EntityEntry entry = _context.Entry(oldValue);
                entry.CurrentValues.SetValues(entity);
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            T? removedEntity = _context.Set<T>().FirstOrDefault(e => e.Id == id);

            if (removedEntity == null)
            {
                throw new ArgumentNullException("There is no entity with this id.");
            }

            _context.Remove(removedEntity);

            await _context.SaveChangesAsync();
        }
    }
}