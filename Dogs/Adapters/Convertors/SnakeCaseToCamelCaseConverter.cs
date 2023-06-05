using Adapters.Abstractions;
using System;
using System.Text;

namespace Adapters.Convertors
{
    public class SnakeCaseToCamelCaseConverter : IConverter<string, string>
    {
        public Task<string> ConvertAsync(string source)
        {
            return Task.FromResult(
                source.Aggregate(new StringBuilder(), (sb, value) =>
                {
                    if (sb.Length == 0)
                    {
                        sb.Append(char.ToUpper(value));
                    } else if (sb[sb.Length - 1] == '_')
                    {
                        sb.Remove(sb.Length - 1, 1);
                        sb.Append(char.ToUpper(value));
                    } else
                    {
                        sb.Append(value);
                    }
                    return sb;
                }).ToString()
            );
        }
    }
}