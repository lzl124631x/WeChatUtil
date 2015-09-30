using System;
using System.Net.Http;
using System.Threading.Tasks;
using WeChatUtil.Models;
using Newtonsoft.Json;

namespace WeChatUtil
{
    public static class WeChatHelper
    {
        public static long NowTimeStamp()
        {
            return DateTimeToTimeStamp(DateTime.UtcNow);
        }

        public static long GetEndTime(long expireDuration)
        {
            return NowTimeStamp() + expireDuration * 1000;
        }

        public static Int64 DateTimeToTimeStamp(DateTime time)
        {
            // .5 is for rounding up.
            return (Int64)((time - new DateTime(1970, 1, 1)).TotalMilliseconds + .5);
        }

        public static async Task<string> DownloadStringFromUrlAsync(string url)
        {
            using (var client = new HttpClient())
            {
                return await client.GetStringAsync(url);
            }
        }

        public static async Task<T> DownloadJsonObjectFromUrlAsync<T>(string url) where T : WeChatJsonBase
        {
            return JsonConvert.DeserializeObject<T>(await DownloadStringFromUrlAsync(url));
        }
    }
}
