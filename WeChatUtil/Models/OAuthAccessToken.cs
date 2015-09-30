using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace WeChatUtil.Models
{
    [DataContract]
    public class OAuthAccessToken : WeChatJsonExpirable
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
        [JsonProperty("expires_in")]
        public long ExpiresIn { get; set; }
        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }
        [JsonProperty("openid")]
        public string OpenId { get; set; }
        [JsonProperty("scope")]
        public string Scope { get; set; }
        [JsonProperty("unionid")]
        public string UnionId { get; set; }
    }
}