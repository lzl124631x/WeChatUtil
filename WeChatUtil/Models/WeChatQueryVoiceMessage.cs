using System.Xml.Serialization;

namespace WeChatUtil.Models
{
    [XmlRoot("xml")]
    public class WeChatQueryVoiceMessage : WeChatQueryMessage
    {
        public CData MediaId { get; set; }
        public CData Format { get; set; }

        public string Recognition { get; set; }

        public WeChatQueryVoiceMessage()
        {
            SetMsgType(MsgTypeEnum.Voice);
        }

        public new static WeChatQueryVoiceMessage LoadFromXmlString(string xml)
        {
            return WeChatMessageExtender.LoadFromXmlString<WeChatQueryVoiceMessage>(xml);
        }
    }
}