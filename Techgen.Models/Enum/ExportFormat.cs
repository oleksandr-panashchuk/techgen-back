using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Techgen.Models.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ExportFormat
    {
        Pdf,
        Xls
    }
}
