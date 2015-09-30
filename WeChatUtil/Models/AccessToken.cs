using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace WeChatUtil.Models
{
    [DataContract]
    public class AccessToken : WeChatJsonExpirable
    {
        [JsonProperty("access_token")]
        public string Token { get; set; }

        [JsonProperty("expires_in")]
        public long ExpiresIn { get; set; }
    }
}