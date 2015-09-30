using System.Xml.Serialization;

namespace WeChatUtil.Models
{
    [XmlRoot("xml")]
    public class WeChatQueryShortVideoMessage : WeChatQueryMessage
    {
        public CData MediaId { get; set; }
        public CData ThumbMediaId { get; set; }

        public WeChatQueryShortVideoMessage()
        {
            SetMsgType(MsgTypeEnum.ShortVideo);
        }

        public new static WeChatQueryShortVideoMessage LoadFromXmlString(string xml)
        {
            return WeChatMessageExtender.LoadFromXmlString<WeChatQueryShortVideoMessage>(xml);
        }
    }
}