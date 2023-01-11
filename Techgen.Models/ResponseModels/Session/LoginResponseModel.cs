using Newtonsoft.Json;

namespace Techgen.Models.ResponseModels.Session
{
    public class LoginResponseModel
    {
        [JsonProperty("user")]
        public UserRoleResponseModel User { get; set; }

        [JsonRequired]
        [JsonProperty("token", NullValueHandling = NullValueHandling.Ignore)]
        public TokenResponseModel Token { get; set; }
    }
}