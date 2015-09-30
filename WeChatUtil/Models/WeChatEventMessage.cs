using System.Collections.Generic;
using System.Xml.Serialization;

namespace WeChatUtil.Models
{
    [XmlRoot("xml")]
    public class WeChatEventMessage : WeChatMessage
    {
        public CData Event;
        public CData EventKey;
        public CData Ticket;
        public double Latitude;
        public double Longitude;
        public double Precision;

        // scancode_push

        private WeChatEventMessage()
        {
            SetMsgType(MsgTypeEnum.Event);
        }

        public new static WeChatEventMessage LoadFromXmlString(string xml)
        {
            return WeChatMessageExtender.LoadFromXmlString<WeChatEventMessage>(xml);
        }

        #region For EventType
        protected void SetEventType(EventTypeEnum type)
        {
            Event = type.ParseToString();
        }

        public EventTypeEnum GetEventType()
        {
            return EventTypeEnumExtender.LoadFromString(Event);
        }
        #endregion
    }

    public class ScanCodeInfo
    {
        public CData ScanType;
        public CData ScanResult;
    }

    public class SendPicsInfo
    {
        public int Count;
        public List<PicItem> PicList;
        public class PicItem
        {
            public CData PicMd5Sum;
        }
    }

    public class SendLocationInfo
    {
        public CData Location_X;
        public CData Location_Y;
        public CData Scale;
        public CData Label;
        public CData PoiName;
    }
}
