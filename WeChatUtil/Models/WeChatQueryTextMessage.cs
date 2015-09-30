using System.Xml.Serialization;

namespace WeChatUtil.Models
{
    [XmlRoot("xml")]
    public class WeChatQueryTextMessage : WeChatQueryMessage
    {
        public CData Content { get; set; }

        public WeChatQueryTextMessage()
        {
            SetMsgType(MsgTypeEnum.Text);
        }

        public new static WeChatQueryTextMessage LoadFromXmlString(string xml)
        {
            return WeChatMessageExtender.LoadFromXmlString<WeChatQueryTextMessage>(xml);
        }
    }
}