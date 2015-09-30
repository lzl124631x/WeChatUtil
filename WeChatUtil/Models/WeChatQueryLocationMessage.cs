using System.Xml.Serialization;

namespace WeChatUtil.Models
{
    [XmlRoot("xml")]
    public class WeChatQueryLocationMessage : WeChatQueryMessage
    {
        public double Location_X { get; set; }
        public double Location_Y { get; set; }
        public int Scale { get; set; }
        public CData Label { get; set; }

        public WeChatQueryLocationMessage()
        {
            SetMsgType(MsgTypeEnum.Location);
        }

        public new static WeChatQueryLocationMessage LoadFromXmlString(string xml)
        {
            return WeChatMessageExtender.LoadFromXmlString<WeChatQueryLocationMessage>(xml);
        }
    }
}