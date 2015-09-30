using System.Collections.Generic;
using System.Xml.Serialization;

namespace WeChatUtil.Models
{
    public class WeChatArticle
    {
        public CData Title { get; set; }
        public CData Description { get; set; }
        public CData PicUrl { get; set; }
        public CData Url { get; set; }
    }

    [XmlRoot("xml")]
    public class WeChatReplyNewsMessage : WeChatReplyMessage
    {
        public int ArticleCount { get; set; }

        [XmlArrayItem("item")]
        public List<WeChatArticle> Articles { get; set; }

        public WeChatReplyNewsMessage()
        {
            SetMsgType(MsgTypeEnum.News);
        }

        public void UpdateArticleCount()
        {
            ArticleCount = Articles != null ? Articles.Count : 0;
        }
    }
}