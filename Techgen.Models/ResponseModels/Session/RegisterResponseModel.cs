using Newtonsoft.Json;

namespace Techgen.Models.ResponseModels.Session
{
    public class RegisterResponseModel
    {
        [JsonRequired]
        [JsonProperty("email")]
        public string Email { get; set; }
    }
}