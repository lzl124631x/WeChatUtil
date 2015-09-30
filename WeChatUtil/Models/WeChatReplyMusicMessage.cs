using System.Xml.Serialization;

namespace WeChatUtil.Models
{
    public class WeChatMusic
    {
        public CData Title { get; set; }
        public CData Desciption { get; set; }
        public CData MusicUrl { get; set; }
        public CData HQMusicUrl { get; set; }
        public CData ThumbMediaId { get; set; }
    }

    [XmlRoot("xml")]
    public class WeChatReplyMusicMessage : WeChatReplyMessage
    {
        public WeChatMusic Music { get; set; }
        public WeChatReplyMusicMessage()
        {
            SetMsgType(MsgTypeEnum.Music);
        }
    }
}