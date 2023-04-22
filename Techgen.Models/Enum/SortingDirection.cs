using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Techgen.Models.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum SortingDirection
    {
        Asc,
        Desc
    }
}
