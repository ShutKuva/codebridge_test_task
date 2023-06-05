using AutoMapper;
using Bll.Abstractions;
using Core;
using Core.Db;
using Core.Dtos;
using Moq;
using System.Linq.Expressions;
using Xunit;

namespace Adapters.Tests
{
    public class DogsAdapterTests
    {
        [Fact]
        public async Task AddDog_AddingValidDog_CompletesSuccessfully()
        {
            Mock<IMapper> mapper = new Mock<IMapper>();
            mapper.Setup(m => m.Map<Dog>(It.IsAny<DogInfo>())).Returns(new Dog
            {
                Id = 0,
                Color = "red",
                Name = "Test",
                TailLength = 10,
                Weight = 10
            });
            Mock<ICrudService<Dog, int, Page>> crudService = new Mock<ICrudService<Dog, int, Page>>();
            var dog = new DogInfo
            {
                Color = "red",
                Name = "Test",
                TailLength = 10,
                Weight = 10
            };
            DogsAdapter dogsAdapter = new DogsAdapter(crudService.Object, mapper.Object);

            Exception ex = await Record.ExceptionAsync(() => dogsAdapter.AddDogAsync(dog));

            Assert.Null(ex);
        }

        [Fact]
        public async Task AddDog_AddingDogWithAlreadyExistName_ThrowsArgumentException()
        {
            Mock<IMapper> mapper = new Mock<IMapper>();
            mapper.Setup(m => m.Map<Dog>(It.IsAny<DogInfo>())).Returns(new Dog
            {
                Id = 0,
                Color = "red",
                Name = "Test",
                TailLength = 10,
                Weight = 10
            });
            Mock<ICrudService<Dog, int, Page>> crudService = new Mock<ICrudService<Dog, int, Page>>();
            crudService
                .Setup(
                    cs => cs.ReadAsync<int>(
                        It.IsAny<Expression<Func<Dog, bool>>>(),
                        It.IsAny<Page>(), null, null))
                    .Returns(
                        Task.FromResult(new List<Dog> { new Dog() }.AsEnumerable())
                    );
            var dog = new DogInfo
            {
                Color = "red",
                Name = "Test",
                TailLength = 10,
                Weight = 10
            };
            DogsAdapter dogsAdapter = new DogsAdapter(crudService.Object, mapper.Object);

            Exception aex = await Record.ExceptionAsync(async () => await dogsAdapter.AddDogAsync(dog));

            Assert.NotNull( aex as ArgumentException );
        }

        [Fact]
        public async Task GetDogs_PassingAsArgumentValidPropertyName_CrudReadAsyncMethodWasCalled()
        {
            Mock<IMapper> mapper = new Mock<IMapper>();
            Mock<ICrudService<Dog, int, Page>> crudService = new Mock<ICrudService<Dog, int, Page>>();
            DogsAdapter dogsAdapter = new DogsAdapter(crudService.Object, mapper.Object);

            await dogsAdapter.GetDogsAsync("name", "", null, null);

            crudService.Verify(cs => cs.ReadAsyncWithUnknownKeyInCompilationTime(null, It.IsAny<Page>(), It.IsAny<LambdaExpression>(), It.IsAny<Type>(), false), Times.Once);
        }

        [Fact]
        public async Task GetDogs_PassingAsArgumentInvalidPropertyName_ThrowsArgumentException()
        {
            Mock<IMapper> mapper = new Mock<IMapper>();
            Mock<ICrudService<Dog, int, Page>> crudService = new Mock<ICrudService<Dog, int, Page>>();
            DogsAdapter dogsAdapter = new DogsAdapter(crudService.Object, mapper.Object);

            Exception ex = await Record.ExceptionAsync(async () => await dogsAdapter.GetDogsAsync("something", "", null, null));

            Assert.NotNull(ex as ArgumentException);
        }
    }
}