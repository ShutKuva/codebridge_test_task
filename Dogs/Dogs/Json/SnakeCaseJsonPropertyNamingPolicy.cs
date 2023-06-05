using System.Text;
using System.Text.Json;

namespace Dogs.Json
{
    public class SnakeCaseJsonPropertyNamingPolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name)
        {
            StringBuilder result = name
                .Select(x => char.IsAsciiLetterUpper(x) ? "_" + x.ToString().ToLower() : x.ToString())
                .Aggregate(new StringBuilder(), (x, v) => x.Append(v));

            if (result[0] == '_')
            {
                result.Remove(0, 1);
            }

            return result.ToString();
        }
    }
}
