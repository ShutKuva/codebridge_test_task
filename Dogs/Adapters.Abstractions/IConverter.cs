namespace Adapters.Abstractions
{
    public interface IConverter<TSource, TResult>
    {
        Task<TResult> ConvertAsync(TSource source);
    }
}