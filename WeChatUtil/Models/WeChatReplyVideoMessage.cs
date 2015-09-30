using System.Xml.Serialization;

namespace WeChatUtil.Models
{
    public class WeChatVideo
    {
        public CData MediaId { get; set; }
        public CData Title { get; set; }
        public CData Description { get; set; }
    }

    [XmlRoot("xml")]
    public class WeChatReplyVideoMessage : WeChatReplyMessage
    {
        public WeChatVideo Video { get; set; }

        public WeChatReplyVideoMessage()
        {
            SetMsgType(MsgTypeEnum.Video);
        }
    }
}