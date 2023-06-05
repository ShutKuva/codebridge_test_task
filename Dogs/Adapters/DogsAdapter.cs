using Adapters.Abstractions;
using Adapters.Convertors;
using AutoMapper;
using Bll.Abstractions;
using Core;
using Core.Db;
using Core.Dtos;
using System.Linq.Expressions;

namespace Adapters
{
    public class DogsAdapter : IDogsAdapter
    {
        private static readonly Func<string, Type> GetPropertyType;

        private readonly ICrudService<Dog, int, Page> _dogCrudService;
        private readonly IMapper _mapper;

        static DogsAdapter()
        {
            ParameterExpression propertyName = Expression.Parameter(typeof(string), "propertyName");

            MethodCallExpression getProperty = Expression.Call(
                    Expression.Constant(typeof(Dog), typeof(Type)),
                    typeof(Type).GetMethods().First(m => m.Name == "GetProperty" && m.GetParameters().Length == 1),
                    propertyName);

            GetPropertyType = Expression.Lambda<Func<string, Type>>(
                Expression.Property(getProperty, "PropertyType"),
                propertyName)
                .Compile();
        }

        public DogsAdapter(ICrudService<Dog, int, Page> dogCrudService, IMapper mapper)
        {
            _dogCrudService = dogCrudService;
            _mapper = mapper;
        }

        public async Task AddDogAsync(DogInfo dog)
        {
            Dog dogForDb = _mapper.Map<Dog>(dog);

            IEnumerable<Dog> dogsWithSameName = await _dogCrudService.ReadAsync<int>(d => d.Name == dogForDb.Name, new Page(), null);

            if (dogsWithSameName.Any())
            {
                throw new ArgumentException("Dog with this name already exists.");
            }

            await _dogCrudService.CreateAsync(dogForDb);
        }

        public async Task<IEnumerable<Dog>> GetDogsAsync(string? attribute, string? order, int? pageNumber, int? pageSize)
        {
            bool isDescending = order == "desc";

            Page page = new Page();
            page.NumberOfEntitiesOnPage = pageSize ?? page.NumberOfEntitiesOnPage;
            page.PageNumber = pageNumber ?? page.PageNumber;

            Type? typeOfProperty = null;
            LambdaExpression orderKey = null;
            if (attribute != null)
            {
                try
                {
                    IConverter<string, string> converter = new SnakeCaseToCamelCaseConverter();

                    string actualName = await converter.ConvertAsync(attribute);

                    typeOfProperty = GetPropertyType(actualName);
                    orderKey = GetPropertyExpression(actualName);
                } catch (NullReferenceException)
                {
                    throw new ArgumentException("There is no property with this name.");
                }
            }

            return await _dogCrudService.ReadAsyncWithUnknownKeyInCompilationTime(null, page, orderKey, typeOfProperty, isDescending);
        }

        private static LambdaExpression GetPropertyExpression(string attribute)
        {
            ParameterExpression entityParameter = Expression.Parameter(typeof(Dog), "entity");
            MemberExpression property = Expression.Property(entityParameter, attribute);
            return Expression.Lambda(property, entityParameter);
        }
    }
}