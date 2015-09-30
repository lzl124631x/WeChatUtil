using System.Xml.Serialization;

namespace WeChatUtil.Models
{
    public class WeChatVoice
    {
        public CData MediaId { get; set; }
    }

    [XmlRoot("xml")]
    public class WeChatReplyVoiceMessage : WeChatReplyMessage
    {
        public WeChatVoice Voice { get; set; }

        public WeChatReplyVoiceMessage()
        {
            SetMsgType(MsgTypeEnum.Voice);
        }
    }
}