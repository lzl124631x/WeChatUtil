using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace WeChatUtil.Models
{
    [DataContract]
    public class WeChatServerIp : WeChatJsonBase
    {
        [JsonProperty("ip_list")]
        public List<string> IpList;
    }
}
