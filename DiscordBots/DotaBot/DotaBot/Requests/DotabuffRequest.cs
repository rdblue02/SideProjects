using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DotaBot.Requests
{
   public abstract class DotabuffRequest<TResponse>
    {     
        protected abstract string Path { get; set; }
        private string path;
        public async Task<(TId id,TResponse response)> Send<TId>(TId id,HttpClient client)
        {
            if (string.IsNullOrEmpty(Path))
                throw new ArgumentNullException(nameof(Path));

            return (id, await Send(client));
        }
        public async Task<TResponse> Send(HttpClient client)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://www.dotabuff.com{Path}");
            request.Headers.Add("authority", new List<string>() { "www.dotabuff.com" });
            request.Headers.Add("scheme", new List<string>() { "https" });
            request.Headers.Add("path", new List<string>() { Path });
            request.Headers.Add("pragma", new List<string>() { "no-cache" });
            request.Headers.Add("cache-control", new List<string>() { "no-cache" });
            request.Headers.Add("sec-ch-ua", new List<string>() { "\" Not A; Brand\";v=\"99\", \"Chromium\";v=\"90\", \"Google Chrome\";v=\"90\"" });
            request.Headers.Add("sec-ch-ua-mobile", new List<string>() { "?0" });
            request.Headers.Add("upgrade-insecure-requests", new List<string>() { "1" });
            request.Headers.Add("user-agent", new List<string>() { "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/90.0.4430.93 Safari/537.36" });
            request.Headers.Add("accept", new List<string>() { "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9" });
            request.Headers.Add("sec-fetch-site", new List<string>() { "none" });
            request.Headers.Add("sec-fetch-mode", new List<string>() { "navigate" });
            request.Headers.Add("sec-fetch-user", new List<string>() { "?1" });
            request.Headers.Add("sec-fetch-dest", new List<string>() { "document" });
            request.Headers.Add("accept-encoding", new List<string>() { "gzip, deflate, br" });
            request.Headers.Add("accept-language", new List<string>() { "en-US,en;q=0.9" });

            var content = await client.SendAsync(request);
            return await ParseServerResponse(content);
        }
        protected abstract Task<TResponse> ParseServerResponse(HttpResponseMessage message);
     
       
    }
}
