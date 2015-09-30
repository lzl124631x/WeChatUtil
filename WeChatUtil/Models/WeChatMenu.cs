using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace WeChatUtil.Models
{
    [DataContract]
    public enum WeChatButtonTypeEnum
    {
        [EnumMember(Value = "click")]
        Click = 1,
        [EnumMember(Value = "view")]
        View,
        [EnumMember(Value = "scancode_push")]
        ScanCodePush,
        [EnumMember(Value = "scancode_waitmsg")]
        ScanCodeWaitMsg,
        [EnumMember(Value = "pic_sysphoto")]
        PicSysPhoto,
        [EnumMember(Value = "pic_photo_or_album")]
        PicPhotoOrAlbum,
        [EnumMember(Value = "pic_weixin")]
        PicWeiXin,
        [EnumMember(Value = "location_select")]
        LocationSelection,
        [EnumMember(Value = "media_id")]
        MediaId,
        [EnumMember(Value = "view_limited")]
        ViewLimited
    }

    [DataContract]
    public class WeChatMenu
    {
        [JsonProperty("button")]
        public List<WeChatButton> Buttons;

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this,
                new JsonSerializerSettings
                {
                    DefaultValueHandling = DefaultValueHandling.Ignore
                });
        }

        public static WeChatMenu LoadFromString(string json)
        {
            return JsonConvert.DeserializeObject<WeChatMenu>(json);
        }
    }

    [DataContract]
    public class WeChatButton
    {
        [JsonProperty("type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public WeChatButtonTypeEnum Type;
        [JsonProperty("name")]
        public string Name;
        [JsonProperty("url")]
        public string Url;
        [JsonProperty("key")]
        public string Key;
        [JsonProperty("media_id")]
        public string MediaId;
        [JsonProperty("sub_button")]
        public List<WeChatButton> SubButttons;
    }

    public class WeChatMenuSetting
    {
        [JsonProperty("is_menu_open")]
        public int IsMenuOpen;
        [JsonProperty("selfmenu_info")]
        public WeChatMenu SelfMenuInfo;
    }
}