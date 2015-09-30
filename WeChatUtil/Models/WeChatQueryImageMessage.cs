using System.Xml.Serialization;

namespace WeChatUtil.Models
{
    [XmlRoot("xml")]
    public class WeChatQueryImageMessage : WeChatQueryMessage
    {
        public CData PicUrl { get; set; }
        public CData MediaId { get; set; }

        public WeChatQueryImageMessage()
        {
            SetMsgType(MsgTypeEnum.Image);
        }

        public new static WeChatQueryImageMessage LoadFromXmlString(string xml)
        {
            return WeChatMessageExtender.LoadFromXmlString<WeChatQueryImageMessage>(xml);
        }
    }
}