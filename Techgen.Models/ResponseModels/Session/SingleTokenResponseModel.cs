using Newtonsoft.Json;

namespace Techgen.Models.ResponseModels.Session
{
    public class SingleTokenResponseModel
    {
        [JsonProperty("token")]
        public string Token { get; set; }
    }
}
