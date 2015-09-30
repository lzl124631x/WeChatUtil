using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace WeChatUtil.Models
{
    [DataContract]
    public class WeChatJsonBase
    {
        [JsonProperty("errcode")]
        public int ErrorCode { get; set; }

        [JsonProperty("errmsg")]
        public string ErrorMessage { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this,
                new JsonSerializerSettings
                {
                    DefaultValueHandling = DefaultValueHandling.Ignore
                });
        }
    }

    [DataContract]
    public class WeChatJsonExpirable : WeChatJsonBase
    {
        // Added by Richard, to label the expiration time.
        [JsonProperty("end_time")]
        public long EndTime { get; set; }
    }
}
