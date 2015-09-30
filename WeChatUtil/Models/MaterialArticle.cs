using Newtonsoft.Json;

namespace WeChatUtil.Models
{
    public class MaterialArticle
    {
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("thumb_media_id")]
        public string ThumbMediaId { get; set; }
        [JsonProperty("author")]
        public string Author { get; set; }
        [JsonProperty("digest")]
        public string Digest { get; set; }
        [JsonProperty("show_cover_pic")]
        public string ShowCoverpic { get; set; }
        [JsonProperty("content")]
        public string Content { get; set; }
        [JsonProperty("content_source_url")]
        public string ContentSourceUrl { get; set; }
    }
}
