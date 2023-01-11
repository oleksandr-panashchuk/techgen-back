using Newtonsoft.Json;

namespace Techgen.Models.ResponseModels
{
    public class CheckResetPasswordTokenResponseModel
    {
        [JsonRequired]
        [JsonProperty("isValid")]
        public bool IsValid { get; set; }
    }
}