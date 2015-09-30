using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net;
using System.Threading;
using System.Diagnostics;
using WeChatUtil.Models;

namespace WeChatUtil
{
    public abstract class WeChatUtility
    {
        public string AppId { get { return _appId; } }
        private readonly string _appId;
        private string AppSecret { get { return _appSecret; } }
        private readonly string _appSecret;
        private string Token { get { return _token; } }
        private readonly string _token;

        private readonly string _wechatAccessTokenKey;
        private readonly string _jsApiTicketKey;
        private readonly string _oAuthAccessTokenKeyPrefix;

        protected abstract Task SaveStringAsync(string key, string data);
        protected abstract Task<string> GetStringAsync(string key);

        protected WeChatUtility(string appId, string appSecret, string token)
        {
            _appId = appId;
            _appSecret = appSecret;
            _token = token;
            _wechatAccessTokenKey = AppId + "-WechatAccessToken";
            _jsApiTicketKey = AppId + "-JsApiTicket";
            _oAuthAccessTokenKeyPrefix = AppId + "-OAuthAccessToken-";
            if (String.IsNullOrEmpty(AppId) || String.IsNullOrEmpty(AppSecret))
            {
                throw new Exception("AppId, AppSecret or data saver/loader methods are not set properly.");
            }
        }

        #region Helper

        private static string CreateNonceStr(int length = 16)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var sb = new StringBuilder();
            for (int i = 0; i < length; ++i)
            {
                sb.Append(chars[RandomGenerator.Instance.Next(0, chars.Length)]);
            }
            return sb.ToString();
        }

        private static string GetSha1Hash(string input)
        {
            using (SHA1Managed sha1 = new SHA1Managed())
            {
                var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
                var sb = new StringBuilder(hash.Length * 2);
                foreach (byte b in hash)
                {
                    sb.Append(b.ToString("x2"));
                }
                return sb.ToString();
            }
        }

        private async Task SaveJsonObjectAsync(string key, object data)
        {
            if (key == null || data == null) return;
            await SaveStringAsync(key, JsonConvert.SerializeObject(data));
        }

        private async Task<T> GetJsonObjectAsync<T>(string key)
        {
            if (key == null) return default(T);
            var json = await GetStringAsync(key);
            return json == null ? default(T) : JsonConvert.DeserializeObject<T>(json);
        }

        #endregion

        #region AccessToken
        protected async Task<string> AccessToken()
        {
            return (await GetAccessTokenAsync()).Token;
        }

        // TODO: make this protected.
        public async Task<AccessToken> GetAccessTokenAsync()
        {
            var token = await GetJsonObjectAsync<AccessToken>(_wechatAccessTokenKey);
            if (token == null || token.EndTime < WeChatHelper.NowTimeStamp())
            {
                token = await RetrieveAndSaveAccessToken();
            }

            return token;
        }

        private async Task<AccessToken> RetrieveAndSaveAccessToken()
        {
            var token = await RetrieveAccessTokenAsync();
            await SaveAccessTokenAsync(token);
            return token;
        }

        private async Task<AccessToken> RetrieveAccessTokenAsync()
        {
            var url =
                string.Format(
                    "https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={0}&secret={1}",
                    AppId, AppSecret);
            return await WeChatHelper.DownloadJsonObjectFromUrlAsync<AccessToken>(url);
        }

        private async Task SaveAccessTokenAsync(AccessToken token)
        {
            if (token != null && token.ErrorCode == 0)
            {
                token.EndTime = WeChatHelper.GetEndTime(token.ExpiresIn);
                await SaveJsonObjectAsync(_wechatAccessTokenKey, token);
            }
        }
        #endregion

        #region JsApiTicket
        public async Task<JsApiTicket> GetJsApiTicketAsync()
        {
            var ticket = await GetJsonObjectAsync<JsApiTicket>(_jsApiTicketKey);
            if (ticket == null || ticket.EndTime < WeChatHelper.NowTimeStamp())
            {
                ticket = await RetrieveAndSaveJsApiTicketAsync();
            }
            return ticket;
        }

        private async Task<JsApiTicket> RetrieveAndSaveJsApiTicketAsync()
        {
            var ticket = await RetrieveJsApiTicketAsync();
            await SaveJsApiTicketAsync(ticket);
            return ticket;
        }

        private async Task<JsApiTicket> RetrieveJsApiTicketAsync()
        {
            var url =
                string.Format(
                    "https://api.weixin.qq.com/cgi-bin/ticket/getticket?access_token={0}&type=jsapi", await AccessToken());
            return await WeChatHelper.DownloadJsonObjectFromUrlAsync<JsApiTicket>(url);
        }

        private async Task SaveJsApiTicketAsync(JsApiTicket ticket)
        {
            if (ticket != null && ticket.ErrorCode == 0)
            {
                ticket.EndTime = WeChatHelper.GetEndTime(ticket.ExpiresIn);
                await SaveJsonObjectAsync(_jsApiTicketKey, ticket);
            }
        }

        #endregion

        #region JsApi SignPackage
        public async Task<SignPackage> GetSignPackageAsync(string signUrl)
        {
            var jsApiTicket = (await GetJsApiTicketAsync()).Ticket;
            if (string.IsNullOrEmpty(jsApiTicket)) // Failed to get JsApiTicket.
            {
                return new SignPackage();
            }
            var timeStamp = Convert.ToString(WeChatHelper.NowTimeStamp());
            var nonceStr = CreateNonceStr();
            var url = signUrl;
            var concat = string.Format("jsapi_ticket={0}&noncestr={1}&timestamp={2}&url={3}", jsApiTicket, nonceStr,
                timeStamp, url);
            var signature = GetSha1Hash(concat);

            return new SignPackage()
            {
                AppId = AppId,
                NonceStr = nonceStr,
                Timestamp = timeStamp,
                Signature = signature
            };
        }
        #endregion

        #region Signature
        // Verify whether requests are sent by WeChat Server.
        public bool CheckSignature(string signature, string timestamp, string nonce)
        {
            return ComputeSignature(timestamp, nonce).Equals(signature, StringComparison.InvariantCultureIgnoreCase);
        }

        //TODO: public is for testing.
        public string ComputeSignature(string timestamp, string nonce)
        {
            List<String> tokens = new List<string>() { Token, timestamp, nonce };
            tokens.Sort();
            var check = String.Concat(tokens);

            byte[] rawCheck = Encoding.UTF8.GetBytes(check);
            SHA1 sha1 = new SHA1Cng();
            byte[] rawHash = sha1.ComputeHash(rawCheck);
            StringBuilder sb = new StringBuilder();
            foreach (byte b in rawHash)
                sb.Append(b.ToString("X2"));
            String hash = sb.ToString();
            return hash;
        }

        #endregion

        #region OAuth
        public async Task<OAuthAccessToken> InitOAuthAccessTokenAsync(string code)
        {
            var url =
                string.Format(
                    "https://api.weixin.qq.com/sns/oauth2/access_token?appid={0}&secret={1}&code={2}&grant_type=authorization_code",
                    AppId, AppSecret, code);
            var token = await RetrieveAndSaveOAuthAccessToken(url);
            await SaveOAuthAccessToken(token);
            return token;
        }

        public async Task<OAuthAccessToken> GetOAuthAccessTokenAsync(string openId)
        {
            var token = await GetJsonObjectAsync<OAuthAccessToken>(_oAuthAccessTokenKeyPrefix + openId);
            if (token != null && token.EndTime < WeChatHelper.NowTimeStamp())
            {
                token = await RefreshOAuthAccessTokenAsync(token.RefreshToken);
                await SaveOAuthAccessToken(token);
            }
            return token;
        }

        public async Task<UserInfo> GetOAuthUserInfoAsync(string openId, string lang)
        {
            var token = await GetOAuthAccessTokenAsync(openId);
            string url = string.Format(
                "https://api.weixin.qq.com/sns/userinfo?access_token={0}&openid={1}&lang={2}",
                token.AccessToken, token.OpenId, lang);
            return await WeChatHelper.DownloadJsonObjectFromUrlAsync<UserInfo>(url);
        }

        private async Task<OAuthAccessToken> RefreshOAuthAccessTokenAsync(string oAuthRefreshToken)
        {
            string url =
                string.Format(
                    "https://api.weixin.qq.com/sns/oauth2/refresh_token?appid={0}&grant_type=refresh_token&refresh_token={1}",
                    AppId, oAuthRefreshToken);
            return await RetrieveAndSaveOAuthAccessToken(url);
        }

        private async Task<OAuthAccessToken> RetrieveAndSaveOAuthAccessToken(string url)
        {
            var token = await RetrieveOAuthAccessToken(url);
            if (token != null && token.ErrorCode == 0)
            {
                await SaveOAuthAccessToken(token);
            }
            return token;
        }

        private async Task<OAuthAccessToken> RetrieveOAuthAccessToken(string url)
        {
            return await WeChatHelper.DownloadJsonObjectFromUrlAsync<OAuthAccessToken>(url);
        }

        private async Task SaveOAuthAccessToken(OAuthAccessToken token)
        {
            if (token != null && token.ErrorCode == 0)
            {
                token.EndTime = WeChatHelper.GetEndTime(token.ExpiresIn);
                await SaveJsonObjectAsync(_oAuthAccessTokenKeyPrefix + token.OpenId, token);
            }
        }
        #endregion

        #region UserInfo

        public async Task<UserInfo> GetUserInfoAsync(string openId, string lang)
        {
            return
                await WeChatHelper.DownloadJsonObjectFromUrlAsync<UserInfo>(
                    string.Format("https://api.weixin.qq.com/cgi-bin/user/info?access_token={0}&openid={1}&lang={2}",
                        (await GetAccessTokenAsync()).Token, openId, lang));
        }

        #endregion

        #region Media

        public async Task<HttpResponseMessage> UploadMediaAsync(MediaTypeEnum msgType, string mediaPath)
        {
            var url = string.Format("https://api.weixin.qq.com/cgi-bin/media/upload?access_token={0}&type={1}",
                await AccessToken(), msgType);
            HttpClient client = new HttpClient();
            var content = new MultipartFormDataContent();
            var stream = new FileStream(mediaPath, FileMode.Open);
            content.Add(new StreamContent(stream));

            // Get the response.
            return await client.PostAsync(url, content);
        }

        public async Task<byte[]> GetMediaFileAsync(string mediaId)
        {
            var url = String.Format("http://file.api.weixin.qq.com/cgi-bin/media/get?access_token={0}&media_id={1}",
                await AccessToken(), mediaId);
            var client = new WebClient();
            var mediaResponse = await client.DownloadDataTaskAsync(url);

            return mediaResponse;
        }
        #endregion

        #region Material

        public static Task UploadMaterialArticleAsync()
        {
            //var url = string.Format("https://api.weixin.qq.com/cgi-bin/material/add_news?access_token={0}",
            //    GetAccessToken());
            throw new NotImplementedException();
        }

        public static Task UploadMaterialAsync()
        {
            //var url = string.Format("https://api.weixin.qq.com/cgi-bin/material/add_material?access_token={0}",
            //    GetAccessToken());
            throw new NotImplementedException();

        }

        public static Task GetMaterialArticleAsync()
        {
            //var url = string.Format("https://api.weixin.qq.com/cgi-bin/material/get_material?access_token={0}",
            //    GetAccessToken());
            throw new NotImplementedException();

        }

        public static Task DeleteMaterialAsync()
        {
            //var url = string.Format("https://api.weixin.qq.com/cgi-bin/material/del_material?access_token={0}",
            //    GetAccessToken());
            throw new NotImplementedException();

        }

        public static Task UpdateMaterialNewsAsync()
        {
            //var url = string.Format("https://api.weixin.qq.com/cgi-bin/material/update_news?access_token={0}",
            //    GetAccessToken());
            throw new NotImplementedException();
        }

        public static Task GetMaterialCountAsync()
        {
            //var url = string.Format("https://api.weixin.qq.com/cgi-bin/material/get_materialcount?access_token={0}",
            //    GetAccessToken());
            throw new NotImplementedException();
        }

        public static Task BatchGetMaterialAsync()
        {
            //var url = string.Format("https://api.weixin.qq.com/cgi-bin/material/batchget_material?access_token={0}", await AccessToken());;
            throw new NotImplementedException();
        }
        #endregion

        #region WeChatMenu

        public async Task<HttpResponseMessage> CreateWeChatMenuAsync(WeChatMenu menu)
        {
            var url = string.Format("https://api.weixin.qq.com/cgi-bin/menu/create?access_token={0}", await AccessToken());
            using (var client = new HttpClient())
            {
                return await client.PostAsync(url, new StringContent(menu.ToString(), Encoding.UTF8, "application/json"));
            }
        }

        public async Task<HttpResponseMessage> GetWeChatMenuAsync()
        {
            var url = string.Format("https://api.weixin.qq.com/cgi-bin/menu/get?access_token={0}", await AccessToken());
            using (var client = new HttpClient())
            {
                return await client.GetAsync(url);
            }
        }

        public async Task<HttpResponseMessage> DeleteWeChatMenuAsync()
        {
            var url = string.Format("https://api.weixin.qq.com/cgi-bin/menu/delete?access_token={0}", await AccessToken());
            using (var client = new HttpClient())
            {
                return await client.GetAsync(url);
            }
        }
        #endregion

        #region WeChatServerIPs

        public async Task<WeChatServerIp> GetWeChatServerIPs()
        {
            var url = string.Format("https://api.weixin.qq.com/cgi-bin/getcallbackip?access_token={0}", await AccessToken());;
            return await WeChatHelper.DownloadJsonObjectFromUrlAsync<WeChatServerIp>(url);
        }
        #endregion

        #region CustomService

        public async Task<HttpResponseMessage> AddCustomServiceAccount(WeChatCustomServiceAccount account)
        {
            var url =
                string.Format("https://api.weixin.qq.com/customservice/kfaccount/add?access_token={0}", await AccessToken());;

            using (var client = new HttpClient())
            {
                return await client.PostAsync(url, new StringContent(account.ToString()));
            }
        }

        public async Task<HttpResponseMessage> ModifyCustomServiceAccount(WeChatCustomServiceAccount account)
        {
            var url = string.Format("https://api.weixin.qq.com/customservice/kfaccount/update?access_token={0}", await AccessToken());;

            using (var client = new HttpClient())
            {
                return await client.PostAsync(url, new StringContent(account.ToString()));
            }
        }

        public async Task<HttpResponseMessage> DeleteCustomServiceAccount(WeChatCustomServiceAccount account)
        {
            var url =
                string.Format("https://api.weixin.qq.com/customservice/kfaccount/del?access_token={0}", await AccessToken());;

            using (var client = new HttpClient())
            {
                return await client.PostAsync(url, new StringContent(account.ToString()));
            }
        }

        public async Task<HttpResponseMessage> SetCustomServiceAccountAvatar(string account)
        {
            var url =
                string.Format(
                    "http://api.weixin.qq.com/customservice/kfaccount/uploadheadimg?access_token={0}&kf_account={1}",
                    await AccessToken(), account);
            using (var client = new HttpClient())
            {
                //return await client.PostAsync(url);
                return null;
            }
        }

        public async Task<HttpResponseMessage> GetAllCustomServiceAccounts()
        {
            var url = string.Format("https://api.weixin.qq.com/cgi-bin/customservice/getkflist?access_token={0}",
                await AccessToken());
            using (var client = new HttpClient())
            {
                return await client.GetAsync(url);
            }
        }

        public async Task<HttpResponseMessage> SendMessage(WeChatCustomServiceMessage message)
        {
            var url = string.Format("https://api.weixin.qq.com/cgi-bin/message/custom/send?access_token={0}",
                await AccessToken());
            using (var client = new HttpClient())
            {
                return await client.PostAsync(url, new StringContent(message.ToString(), Encoding.UTF8, "application/json"));
            }
        }
        #endregion

        #region Send Messages

        private static readonly string PostURIFormat = "https://api.weixin.qq.com/cgi-bin/message/custom/send?access_token={0}";
        private static readonly string MediaPostURIFormat = "http://file.api.weixin.qq.com/cgi-bin/media/upload?access_token={0}&type={1}";
        private static readonly int MessageDelay = 1000;
        public static string SendTextMessage(string toUserID, string content, string accessToken, bool wait = false)
        {
            try
            {
                WechatEasyMessage message = new WechatEasyMessage();
                message.msgtype = "text";
                message.touser = toUserID;
                message.text = new WechatTextMessage() { content = content };

                WebClient wc = new WebClient();

                wc.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

                byte[] postData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

                byte[] responseData = wc.UploadData(string.Format(PostURIFormat, accessToken), "POST", postData);

                if (wait)
                {
                    Thread.Sleep(MessageDelay);
                }

                return Encoding.UTF8.GetString(responseData);// 解码  
            }
            catch (Exception ex)
            {
                Trace.TraceInformation(ex.Message);
                return string.Empty;
            }
        }

        internal static string SendVoiceMessage(string toUserID, byte[] data, string accessToken, bool wait = false)
        {
            var mediaIdResponse = UploadMediaFile(accessToken, "voice", data, "test.amr", "audio/amr");
            try
            {
                var mediaId = mediaIdResponse;

                WechatEasyMessage message = new WechatEasyMessage();
                message.msgtype = "voice";
                message.touser = toUserID;
                message.voice = new WechatMediaMessage() { media_id = mediaId };

                WebClient wc = new WebClient();

                wc.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

                byte[] postData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

                byte[] responseData = wc.UploadData(string.Format(PostURIFormat, accessToken), "POST", postData);
                if (wait)
                {
                    Thread.Sleep(MessageDelay);
                }
                return Encoding.UTF8.GetString(responseData);// 解码   
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
            }

            return string.Empty;
        }

        internal static string SendImageMessage(string toUserID, byte[] data, string accessToken, bool wait = false)
        {
            var mediaIdResponse = UploadMediaFile(accessToken, "image", data, "test.jpg", "image/jpeg");
            try
            {
                var mediaId = mediaIdResponse;

                WechatEasyMessage message = new WechatEasyMessage();
                message.msgtype = "image";
                message.touser = toUserID;
                message.image = new WechatMediaMessage() { media_id = mediaId };

                WebClient wc = new WebClient();

                wc.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

                byte[] postData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

                byte[] responseData = wc.UploadData(string.Format(PostURIFormat, accessToken), "POST", postData);
                if (wait)
                {
                    Thread.Sleep(MessageDelay);
                }
                return Encoding.UTF8.GetString(responseData);// 解码   
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
            }

            return string.Empty;
        }

        public static string UploadMediaFile(string accesstoken, string type, byte[] data, string filename, string contenttype)
        {
            try
            {
                byte[] buffer = data;

                string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
                WebRequest req = WebRequest.Create(string.Format(MediaPostURIFormat, accesstoken, type));
                req.Method = "POST";
                req.ContentType = "multipart/form-data; boundary=" + boundary;

                StringBuilder sb = new StringBuilder();
                sb.Append("--" + boundary + "\r\n");
                sb.Append("Content-Disposition: form-data; name=\"media\"; filename=\"" + filename + "\"; filelength=\"" + data.Length + "\"");
                sb.Append("\r\n");
                sb.Append("Content-Type: " + contenttype);
                sb.Append("\r\n\r\n");
                string head = sb.ToString();
                byte[] form_data = Encoding.UTF8.GetBytes(head);

                byte[] foot_data = Encoding.UTF8.GetBytes("\r\n--" + boundary + "--\r\n");

                long length = form_data.Length + data.Length + foot_data.Length;

                req.ContentLength = length;

                Stream requestStream = req.GetRequestStream();
                requestStream.Write(form_data, 0, form_data.Length);
                requestStream.Write(buffer, 0, buffer.Length);
                requestStream.Write(foot_data, 0, foot_data.Length);

                requestStream.Close();

                WebResponse pos = req.GetResponse();
                string html;
                using (StreamReader sr = new StreamReader(pos.GetResponseStream(), Encoding.UTF8))
                {
                    html = sr.ReadToEnd().Trim();
                }
                if (pos != null)
                {
                    pos.Close();
                    pos = null;
                }
                if (req != null)
                {
                    req = null;
                }

                dynamic x = JsonConvert.DeserializeObject(html);
                var mediaId = x.media_id;
                return mediaId;
            }
            catch (Exception ex)
            {
                Trace.TraceInformation(ex.Message);
                return string.Empty;
            }
        }

        internal static string SendImageMessage(string toUserID, string imagePath, string accessToken, bool wait = false)
        {
            if (!File.Exists(imagePath)) return string.Empty;

            var data = File.ReadAllBytes(imagePath);
            try
            {
                var mediaIdResponse = UploadMediaFile(accessToken, "image", data, "test.jpg", "image/jpeg");

                var mediaId = mediaIdResponse;

                WechatEasyMessage message = new WechatEasyMessage();
                message.msgtype = "image";
                message.touser = toUserID;
                message.image = new WechatMediaMessage() { media_id = mediaId };

                WebClient wc = new WebClient();

                wc.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

                byte[] postData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

                byte[] responseData = wc.UploadData(string.Format(PostURIFormat, accessToken), "POST", postData);
                if (wait)
                {
                    Thread.Sleep(MessageDelay);
                }
                return Encoding.UTF8.GetString(responseData);// 解码   
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
            }

            return string.Empty;
        }

        #endregion
    }
}