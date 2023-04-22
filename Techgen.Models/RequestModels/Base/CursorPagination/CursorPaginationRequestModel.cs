using Newtonsoft.Json;
using Techgen.Models.Enum;
using Techgen.Models.Enums;

namespace Techgen.Models.RequestModels.Base.CursorPagination
{
    public class CursorPaginationRequestModel<T> : CursorPaginationBaseRequestModel where T : struct
    {
        [JsonProperty("search")]
        public string Search { get; set; }

        [JsonProperty("order")]
        public OrderingRequestModel<T, SortingDirection> Order { get; set; }
    }
}
