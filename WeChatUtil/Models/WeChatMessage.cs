using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace WeChatUtil.Models
{
    public enum MsgTypeEnum
    {
        Text,
        Image,
        Voice,
        Video,
        ShortVideo,
        Location,
        Link,
        Music,
        News,
        Thumb,
        Event,
    }

    public enum EventTypeEnum
    {
        Subscribe,
        Unsubscribe,
        Scan,
        Location,
        Click,
        View,
        ScanCodePush,
        ScanCodeWaitMsg,
        PicSysPhoto,
        PicPhotoOrAlbum,
        PicWeiXin,
        LocationSelect
    }

    public static class MsgTypeEnumExtender
    {
        public static string ParseToString(this MsgTypeEnum type)
        {
            return Convert.ToString(type).ToLower();
        }

        public static MsgTypeEnum LoadFromString(string typeStr)
        {
            return (MsgTypeEnum)Enum.Parse(typeof(MsgTypeEnum), typeStr, true);
        }
    }

    public static class EventTypeEnumExtender
    {
        public static string ParseToString(this EventTypeEnum type)
        {
            return Convert.ToString(type).ToUpper();
        }

        public static EventTypeEnum LoadFromString(string typeStr)
        {
            return (EventTypeEnum) Enum.Parse(typeof (EventTypeEnum), typeStr, true);
        }
    }

    public static class WeChatMessageExtender
    {
        public static T LoadFromXmlString<T>(string data) where T : WeChatMessage
        {
            if (string.IsNullOrEmpty(data))
            {
                return default(T);
            }
            var slr = new XmlSerializer(typeof(T));
            T msg;
            using (var sr = new StringReader(data))
            {
                msg = (T)slr.Deserialize(sr);
            }
            return msg;
        }

        public static T BindInput<T>(this T msg, WeChatMessage input) where T : WeChatReplyMessage
        {
            msg.ToUserName = input.FromUserName;
            msg.FromUserName = input.ToUserName;
            return msg;
        }
    }

    [XmlRoot("xml")]
    public class WeChatMessage
    {
        public CData ToUserName { get; set; }
        public CData FromUserName { get; set; }
        public Int64 CreateTime { get; set; }
        public CData MsgType { get; set; }

        public WeChatMessage()
        {
            CreateTime = WeChatHelper.NowTimeStamp();
        }

        #region For MsgType
        protected void SetMsgType(MsgTypeEnum type)
        {
            MsgType = type.ParseToString();
        }

        public MsgTypeEnum GetMsgType()
        {
            return MsgTypeEnumExtender.LoadFromString(MsgType);
        }
        #endregion

        public static WeChatMessage LoadFromXmlString(string msgStr)
        {
            return WeChatMessageExtender.LoadFromXmlString<WeChatMessage>(msgStr);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
        public virtual string GetXmlString()
        {
            var slr = new XmlSerializer(GetType());
            using (StringWriter sw = new StringWriter())
            {
                // xw will not dispose sw (by using CloseOutput flag), so it is safe to suppress CA2202
                using (XmlWriter xw = XmlWriter.Create(sw, new XmlWriterSettings()
                {
                    // Remove Xml Declaration.
                    OmitXmlDeclaration = true,
                    Encoding = Encoding.UTF8,
                    CloseOutput = false,
                }))
                {
                    // Remove Namespace.
                    XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                    ns.Add("", "");
                    slr.Serialize(xw, this, ns);
                    xw.Flush();
                    sw.Flush();
                    return sw.ToString();
                }
            }
        }
    }

    [XmlRoot("xml")]
    public class WeChatQueryMessage : WeChatMessage
    {
        public Int64 MsgId { get; set; }

        public static MsgTypeEnum GetType(string msgStr)
        {
            if (string.IsNullOrWhiteSpace(msgStr))
            {
                throw new ArgumentException("MsgStr can't be empty.", "msgStr");
            }
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(msgStr);
            return MsgTypeEnumExtender.LoadFromString(doc.SelectSingleNode("xml/MsgType").InnerText);
        }
    }

    [XmlRoot("xml")]
    public abstract class WeChatReplyMessage : WeChatMessage
    {
    }

    public class WechatEasyMessage
    {
        public string touser { get; set; }

        public string msgtype { get; set; }

        public WechatTextMessage text { get; set; }
        public WechatMediaMessage image { get; set; }
        public WechatMediaMessage voice { get; set; }
    }

    public class WechatTextMessage
    {
        public string content { get; set; }
    }

    public class WechatMediaMessage
    {
        public string media_id { get; set; }
    }
}