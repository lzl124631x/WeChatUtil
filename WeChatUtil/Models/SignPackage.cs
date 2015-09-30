using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace WeChatUtil.Models
{
    [DataContract]

    public class SignPackage
    {
        [JsonProperty("appId")]
        public string AppId { get; set; }

        [JsonProperty("nonceStr")]
        public string NonceStr { get; set; }

        [JsonProperty("timestamp")]
        public string Timestamp { get; set; }

        [JsonProperty("signature")]
        public string Signature { get; set; }
    }
}