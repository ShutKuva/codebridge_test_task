using Core;
using Core.Db;
using DAL;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BLL.Tests
{
    public class CrudServiceTests : IDisposable
    {
        private readonly DogsContext _context;

        public CrudServiceTests()
        {
            DbContextOptionsBuilder<DogsContext> contextOptionsBuilder = new DbContextOptionsBuilder<DogsContext>();
            contextOptionsBuilder.UseInMemoryDatabase("CrudServiceTests");

            _context = new DogsContext(contextOptionsBuilder.Options);

            _context.Database.EnsureCreated();
        }

        [Fact]
        public async Task CreateAsync_PassingValidDog_DogAdded()
        {
            CrudService<Dog> crudService = new CrudService<Dog>(_context);
            Dog validDog = new Dog
            {
                Name = "Test",
                Color = "brown",
                TailLength = 5,
                Weight = 1,
            };

            await crudService.CreateAsync(validDog);

            Assert.Contains(validDog, _context.Dogs);
        }

        [Fact]
        public async Task CreateAsync_PassingNull_ThrowsArgumentException()
        {
            CrudService<Dog> crudService = new CrudService<Dog>(_context);

            Exception ex = await Record.ExceptionAsync(async () => await crudService.CreateAsync(null));

            Assert.NotNull(ex as ArgumentException);
        }

        [Fact]
        public async Task DeleteAsync_PassingDogThatExistsInDb_DeletesDog()
        {
            CrudService<Dog> crudService = new CrudService<Dog>(_context);
            Dog validDog = new Dog
            {
                Name = "Test",
                Color = "brown",
                TailLength = 5,
                Weight = 1,
            };
            _context.Dogs.Add(validDog);
            await _context.SaveChangesAsync();

            await crudService.DeleteAsync(validDog.Id);

            Assert.DoesNotContain(validDog, _context.Dogs);
        }

        [Fact]
        public async Task UpdateAsync_PassingDogThatExistsInDbWithDifferentFields_UpdatesDog()
        {
            CrudService<Dog> crudService = new CrudService<Dog>(_context);
            Dog validDog = new Dog
            {
                Name = "Test",
                Color = "brown",
                TailLength = 5,
                Weight = 1,
            };
            _context.Dogs.Add(validDog);
            await _context.SaveChangesAsync();

            Dog updatedDog = new Dog
            {
                Id = validDog.Id,
                Name = "Test1",
                TailLength = validDog.TailLength,
                Weight = validDog.Weight,
            };

            await crudService.UpdateAsync(updatedDog);

            Assert.True(_context.Dogs.First().Name == "Test1");
        }

        [Fact]
        public async Task ReadAsync_PassingValidParameters_ReturnsSpecifiedSequence()
        {
            CrudService<Dog> crudService = new CrudService<Dog>(_context);
            Dog test1 = new Dog
            {
                Name = "Test1",
                Color = "brown",
                TailLength = 5,
                Weight = 1,
            };
            Dog test2 = new Dog
            {
                Name = "Test2",
                Color = "brown",
                TailLength = 5,
                Weight = 1,
            };
            _context.Add(test1);
            _context.Add(test2);
            await _context.SaveChangesAsync();

            IEnumerable<Dog> readedDogs = await crudService.ReadAsync<int>(null, new Page(), null, null);

            Assert.Contains(test1, readedDogs);
            Assert.Contains(test2, readedDogs);
        }

        [Fact]
        public async Task ReadAsyncWithUnknownKeyInCompilationTime_PassingValidParameters_ReturnsSpecifiedSequence()
        {
            CrudService<Dog> crudService = new CrudService<Dog>(_context);
            Dog test1 = new Dog
            {
                Name = "Test1",
                Color = "brown",
                TailLength = 5,
                Weight = 1,
            };
            Dog test2 = new Dog
            {
                Name = "Test2",
                Color = "brown",
                TailLength = 5,
                Weight = 1,
            };
            _context.Add(test1);
            _context.Add(test2);
            await _context.SaveChangesAsync();

            IEnumerable<Dog> readedDogs = await crudService.ReadAsyncWithUnknownKeyInCompilationTime(null, new Page(), null, null, null);

            Assert.Contains(test1, readedDogs);
            Assert.Contains(test2, readedDogs);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
        }
    }
}