using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace WeChatUtil.Models
{
    [DataContract]
    class WeChatMaterialCount
    {
        [JsonProperty("errcode")]
        public string ErrorCode { get; set; }
        [JsonProperty("errmsg")]
        public string ErrorMessage { get; set; }
        [JsonProperty("voice_count")]
        public int VoiceCount { get; set; }
        [JsonProperty("video_count")]
        public int VideoCount { get; set; }
        [JsonProperty("image_count")]
        public int ImageCount { get; set; }
        [JsonProperty("news_count")]
        public int NewsCount { get; set; }
    }
}
