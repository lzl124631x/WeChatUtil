using System.Xml.Serialization;

namespace WeChatUtil.Models
{
    public class WeChatImage
    {
        public CData MediaId { get; set; }
    }

    [XmlRoot("xml")]
    public class WeChatReplyImageMessage : WeChatReplyMessage
    {
        public WeChatImage Image { get; set; }

        public WeChatReplyImageMessage()
        {
            SetMsgType(MsgTypeEnum.Image);
        }
    }
}
