using System.Collections.Generic;
using Newtonsoft.Json;

namespace WeChatUtil.Models
{
    public class WeChatCustomServiceAccount : WeChatJsonBase
    {
        [JsonProperty("kf_account")]
        public string Account;
        [JsonProperty("nickname")]
        public string Nickname;
        [JsonProperty("password")]
        public string Password;
    }

    public class WeChatCustomServiceMessage
    {
        [JsonProperty("touser")]
        public string ToUser;
        [JsonProperty("msgtype")]
        public string Type;
        [JsonProperty("customservice")]
        public WeChatCustomServiceAccount CustomeServiceAccount;

        public class WeChatCustomServiceAccount
        {
            [JsonProperty("kf_account")]
            public string Account;
        }
    }

    public class WeChatCustomServiceTextMessage : WeChatCustomServiceMessage
    {
        [JsonProperty("text")]
        public WeChatCustomServiceTextMessageContent Text;

        public class WeChatCustomServiceTextMessageContent
        {
            [JsonProperty("content")]
            public string Content;
        }
    }

    public class WeChatCustomServiceImageMessage : WeChatCustomServiceMessage
    {
        [JsonProperty("image")]
        public WeChatCustomServiceImageMessageContent Image;

        public class WeChatCustomServiceImageMessageContent
        {
            [JsonProperty("media_id")]
            public string MediaId;
        }
    }

    public class WeChatCustomServiceVoiceMessage : WeChatCustomServiceMessage
    {
        [JsonProperty("voice")]
        public WeChatCustomServiceVoiceMessageContent Voice;

        public class WeChatCustomServiceVoiceMessageContent
        {
            [JsonProperty("media_id")]
            public string MediaId;
        }
    }

    public class WeChatCustomServiceVideoMessage : WeChatCustomServiceMessage
    {
        [JsonProperty("video")]
        public WeChatCustomServiceVideoMessageContent Video;

        public class WeChatCustomServiceVideoMessageContent
        {
            [JsonProperty("media_id")]
            public string MediaId;
            [JsonProperty("media_id")]
            public string ThumbMediaId;
            [JsonProperty("title")]
            public string Title;
            [JsonProperty("description")]
            public string Description;
        }
    }

    public class WeChatCustomServiceMusicMessage : WeChatCustomServiceMessage
    {
        [JsonProperty("music")]
        public WeChatCustomServiceMusicMessageContent Music;

        public class WeChatCustomServiceMusicMessageContent
        {
            [JsonProperty("title")]
            public string Title;
            [JsonProperty("description")]
            public string Description;
            [JsonProperty("musicurl")]
            public string MusicUrl;
            [JsonProperty("hqmusicurl")]
            public string HqMusicUrl;
            [JsonProperty("media_id")]
            public string ThumbMediaId;
        }
    }

    public class WeChatCustomServiceNewsMessage : WeChatCustomServiceMessage
    {
        [JsonProperty("news")]
        public WeChatCustomServiceNewsMessageContent News;

        public class WeChatCustomServiceNewsMessageContent
        {
            [JsonProperty("ariticles")]
            public List<WeChatArticle> Articles;
        }
    }

    public class WeChatCustomServiceCardMessage : WeChatCustomServiceMessage
    {
        [JsonProperty("wxcard")]
        public WeChatCustomServiceCardMessageContent Card;

        public class WeChatCustomServiceCardMessageContent
        {
            [JsonProperty("card_id")]
            public string CardId;
            [JsonProperty("card_ext")]
            public string CardExt;

            public class CardData
            {
                public string Code;
                public string OpenId;
                public string TimeStamp;
                public string Signature;
            }
        }
    }
}
