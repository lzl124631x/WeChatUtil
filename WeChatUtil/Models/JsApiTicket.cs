using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace WeChatUtil.Models
{
    [DataContract]

    public class JsApiTicket : WeChatJsonExpirable
    {
        [JsonProperty("ticket")]
        public string Ticket { get; set; }

        [JsonProperty("expires_in")]
        public long ExpiresIn { get; set; }
    }
}