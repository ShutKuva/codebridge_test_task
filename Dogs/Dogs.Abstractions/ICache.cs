namespace Dogs.Abstractions
{
    public interface ICache<TKey>
    {
        public Task<TValue> GetAsync<TValue>(TKey key, TValue defaultValue);
        public Task RemoveAsync(TKey key);
        public Task AddAsync<TValue>(TKey key, TValue value);
        public Task SetAsync<TValue>(TKey key, TValue value);
    }
}