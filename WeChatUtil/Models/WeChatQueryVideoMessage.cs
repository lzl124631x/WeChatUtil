using System.Xml.Serialization;

namespace WeChatUtil.Models
{
    [XmlRoot("xml")]
    public class WeChatQueryVideoMessage : WeChatQueryMessage
    {
        public CData MediaId { get; set; }
        public CData ThumbMediaId { get; set; }

        public WeChatQueryVideoMessage()
        {
            SetMsgType(MsgTypeEnum.Video);
        }

        public new static WeChatQueryVideoMessage LoadFromXmlString(string xml)
        {
            return WeChatMessageExtender.LoadFromXmlString<WeChatQueryVideoMessage>(xml);
        }
    }
}