using System.Xml.Serialization;

namespace WeChatUtil.Models
{
    [XmlRoot("xml")]
    public class WeChatQueryLinkMessage : WeChatQueryMessage
    {
        public CData Title { get; set; }
        public CData Description { get; set; }
        public CData Url { get; set; }

        public WeChatQueryLinkMessage()
        {
            SetMsgType(MsgTypeEnum.Text);
        }

        public new static WeChatQueryLinkMessage LoadFromXmlString(string xml)
        {
            return WeChatMessageExtender.LoadFromXmlString<WeChatQueryLinkMessage>(xml);
        }
    }
}