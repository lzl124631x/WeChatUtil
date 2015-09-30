using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace WeChatUtil.Models
{
    [DataContract]
    public class UserInfo : WeChatJsonBase
    {
        [JsonProperty("openid")]
        public string OpenId { get; set; }
        [JsonProperty("nickname")]
        public string Nickname { get; set; }
        [JsonProperty("sex")]
        public int Sex { get; set; }
        [JsonProperty("language")]
        public string Language { get; set; }
        [JsonProperty("province")]
        public string Province { get; set; }
        [JsonProperty("city")]
        public string City { get; set; }
        [JsonProperty("country")]
        public string Country { get; set; }
        [JsonProperty("headimgurl")]
        public string HeadImgUrl { get; set; }
        [JsonProperty("privilege")]
        public string[] Privilege { get; set; }
        [JsonProperty("unionid")]
        public string UnionId { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}