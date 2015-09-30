using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace WeChatUtil.Models
{
    [XmlRoot("xml")]
    public class WeChatReplyTextMessage : WeChatReplyMessage
    {
        public CData Content { get; set; }

        public WeChatReplyTextMessage()
        {
            SetMsgType(MsgTypeEnum.Text);
        }

        public new static WeChatQueryTextMessage LoadFromXmlString(string xml)
        {
            return WeChatMessageExtender.LoadFromXmlString<WeChatQueryTextMessage>(xml);
        }
    }
}
