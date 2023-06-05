using Adapters.Abstractions;
using Adapters.Convertors;
using Xunit;

namespace Adapters.Tests
{
    public class SnakeCaseToCamelCaseConverterTests
    {
        [Fact]
        public async Task ConvertAsync_PassingString_RetrievingCamelCaseString()
        {
            IConverter<string, string> converter = new SnakeCaseToCamelCaseConverter();
            string testString = "test_snake_case_string";

            string result = await converter.ConvertAsync(testString);

            Assert.Equal("TestSnakeCaseString", result);
        }
    }
}