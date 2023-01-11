using Newtonsoft.Json;

namespace Techgen.Models.ResponseModels
{
    public class IdResponseModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }
    }
}
