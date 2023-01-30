using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Techgen.Models.ResponseModels.Base
{
    public class JsonResponse<T>
    {
        public JsonResponse(T newdata)
        {
            Data = newdata;
        }

        [JsonRequired]
        [JsonProperty("_v")]
        public string Version { get; set; } = "1.0";

        //[JsonRequired]
        [JsonProperty("data")]
        public T Data { get; set; }
    }
}
