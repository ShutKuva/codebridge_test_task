using Core.Db;
using Core.Dtos;

namespace Adapters.Abstractions
{
    public interface IDogsAdapter
    {
        Task<IEnumerable<Dog>> GetDogsAsync(string? attribute, string? order, int? pageNumber, int? pageSize);

        Task AddDogAsync(DogInfo dog);
    }
}