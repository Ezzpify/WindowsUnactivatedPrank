using System.Net;

namespace ServiceHub.Host.Node
{
    class Website
    {
        public static string DownloadString(string url)
        {
            using (var client = new WebClient())
            {
                try
                {
                    client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                    client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                    return client.DownloadString(url);
                }
                catch
                {
                    return string.Empty;
                }
            }
        }
    }
}
