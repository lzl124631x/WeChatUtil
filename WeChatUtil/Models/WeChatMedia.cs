using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace WeChatUtil.Models
{
    public enum MediaTypeEnum
    {
        Image,
        Voice,
        Video,
        Thumb
    }
    [DataContract]
    public class WeChatMedia
    {
        [JsonProperty("errcode")]
        public string ErrorCode { get; set; }
        [JsonProperty("errmsg")]
        public string ErrorMessage { get; set; }
        [JsonProperty("type")]
        public MediaTypeEnum Type { get; set; }
        [JsonProperty("media_id")]
        public string MediaId { get; set; }
        [JsonProperty("created_at")]
        public string CreatedAt { get; set; }
    }
}
